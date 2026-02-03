using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EasyTextEffects;

public delegate void OnDialogueEndEventHandler();

public class DialogueManager : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextEffect dialogueTextEfect;

    [Header("Choices UI")]
    [SerializeField] private GameObject dialogueChoicesParent;
    [SerializeField] private GameObject choicePrefab;
    private GameObject[] choices = { };
    private TextMeshProUGUI[] choicesText = { };
    private int maxChoiceNum = 4;
    private Coroutine typeCoroutine;

    [Header("Speakers")]
    [SerializeField] private GameObject rightSpeaker;
    [SerializeField] private TextMeshProUGUI rightSpeakerDisplayName;
    [SerializeField] private Image rightSpeakerImage;
    [SerializeField] private GameObject leftSpeaker;
    [SerializeField] private TextMeshProUGUI leftSpeakerDisplayName;
    [SerializeField] private Image leftSpeakerImage;
    [SerializeField] private SpeakerObject[] speakers;

    public event OnDialogueEndEventHandler OnDialogueEnd;

    private int actualSpeaker = 0;
    private string actualSprite = "neutral";
    private TextMeshProUGUI actualDisplayName;
    private Image actualSpeakerImage;
    private bool left = true;

    private Story currentStory;

    private static DialogueManager instance;

    public static DialogueManager Instance { get { return instance; } }

    public bool dialogueIsPlaying { get; private set; }
    public int GoodAnswers { get; private set; }

    private bool canContinueToTheNextLine = true;
    private bool buttonClickedWhileTyping = false;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one InputManager in the Scene");
        }

        instance = this;
    }

    private void Start()
    {
        ExitDialogueModeRaw();

        SwitchLayout(left);

        InputManager.Instance.OnNextPressed += NextPressed;
    }

    private void NextPressed()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        if (canContinueToTheNextLine && currentStory.currentChoices.Count == 0)
        {
            ContinueStory();
            return;
        }

        if (!canContinueToTheNextLine)
        {
            buttonClickedWhileTyping = true;
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        actualDisplayName.text = "???";
        actualSprite = "neutral";
        actualSpeaker = 0;
        SwitchLayout(false);
        continueIcon.SetActive(false);

        GoodAnswers = 0;

        currentStory.BindExternalFunction("AddGoodAnswer", (string points) =>
        {
            AddGoodAnswer();
        });

        ContinueStory();
    }

    private void AddGoodAnswer()
    {
        GoodAnswers++;
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        InputManager.Instance.DisableDialogMap();
        InputManager.Instance.EnablePlayerMap();
        ExitDialogueModeRaw();
        OnDialogueEnd?.Invoke();
    }

    private void ExitDialogueModeRaw()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if (typeCoroutine != null) StopCoroutine(typeCoroutine);
            typeCoroutine = StartCoroutine(TypeText(currentStory.Continue()));
            HandleTags(currentStory.currentTags);
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey) 
            {
                case SPEAKER_TAG:
                    int speaker = int.Parse(tagValue);
                    string text = "???";
                    Sprite s = null;
                    if (speaker >= 0 && speaker < speakers.Length)
                    {
                        actualSpeaker = speaker;
                        text = speakers[speaker].displayName;
                        s = speakers[speaker].GetSprite(actualSprite);
                    }
                    actualDisplayName.text = text;
                    actualSpeakerImage.sprite = s;
                    break;
                case PORTRAIT_TAG:
                    actualSpeakerImage.sprite = speakers[actualSpeaker].GetSprite(tagValue);
                    break;
                case LAYOUT_TAG:
                    SwitchLayout(tagValue == "left");
                    break;
                default:
                    Debug.LogError("Tag came in but is not currently being handle: " + tag);
                    break;
            }
        }
    }

    private void SwitchLayout(bool _left)
    {
        left = _left;

        string text = actualDisplayName == null ? "???" : actualDisplayName.text;
        Sprite s = actualSpeakerImage == null ? null : actualSpeakerImage.sprite;

        if (left) 
        {
            rightSpeaker.SetActive(false);
            leftSpeaker.SetActive(true);

            actualDisplayName = leftSpeakerDisplayName;
            actualSpeakerImage = leftSpeakerImage;
        }
        else
        {
            rightSpeaker.SetActive(true);
            leftSpeaker.SetActive(false);

            actualDisplayName = rightSpeakerDisplayName;
            actualSpeakerImage = rightSpeakerImage;
        }

        actualDisplayName.text = text;
        actualSpeakerImage.sprite = s;
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoice = currentStory.currentChoices;

        if (currentChoice.Count == 0) return;

        if (currentChoice.Count > maxChoiceNum)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " + currentChoice.Count);
        }

        int choicesNum = Mathf.Min(currentChoice.Count, maxChoiceNum);
        choices = new GameObject[choicesNum];
        choicesText = new TextMeshProUGUI[choicesNum];

        int index = 0;
        foreach (Choice choice in currentChoice)
        {
            int capturedIndex = index;
            GameObject obj = Instantiate(choicePrefab, dialogueChoicesParent.transform, false);
            obj.GetComponent<Button>().onClick.AddListener(() => { MakeChoice(capturedIndex); });
            choices[index] = obj;
            choices[index].gameObject.SetActive(true);
            choicesText[index] = obj.GetComponentInChildren<TextMeshProUGUI>();
            choicesText[index].text = choice.text;
            index++;
        }
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToTheNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);

            ClearChoices();

            currentStory.Continue();

            if (currentStory.currentText != "\n" && currentStory.currentText != "")
            {
                if (currentStory.canContinue)
                {
                    if (typeCoroutine != null) StopCoroutine(typeCoroutine);
                    typeCoroutine = StartCoroutine(TypeText(currentStory.currentText));
                    HandleTags(currentStory.currentTags);
                }
                else
                {
                    StartCoroutine(ExitDialogueMode());
                }
            }
            else
            {
                ContinueStory();
            }
        }
    }

    private void ClearChoices()
    {
        if (choicesText.Length != 0)
        {
            System.Array.Clear(choicesText, 0, choicesText.Length);
        }
        if (choices.Length != 0)
        {
            foreach (var ch in choices)
            {
                DestroyImmediate(ch.gameObject);
            }
            System.Array.Clear(choices, 0, choices.Length);
        }
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";

        continueIcon.SetActive(false);
        //ClearChoices();

        canContinueToTheNextLine = false;
        bool isAddingRichTextTag = false;

        foreach (char letter in text.ToCharArray())
        {
            if (buttonClickedWhileTyping)
            {
                buttonClickedWhileTyping = false;
                dialogueText.text = text;
                break;
            }

            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                dialogueText.text += letter;

                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            else
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            dialogueTextEfect.Refresh();
        }

        continueIcon.SetActive(true);
        DisplayChoices();

        canContinueToTheNextLine = true;
    }
}