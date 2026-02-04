using UnityEngine;

public class DialogueTriggerTest : MonoBehaviour
{
    public TextAsset dialogueContent;

    private void Start()
    {
        InputManager.Instance.OnNextPressed += () => { StartClicked(); };
    }

    public void StartClicked()
    {
        if (DialogueManager.Instance.dialogueIsPlaying)
        {
            return;
        }

        DialogueManager.Instance.EnterDialogueMode(dialogueContent);
    }
}
