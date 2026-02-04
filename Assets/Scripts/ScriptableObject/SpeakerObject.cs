using UnityEngine;

[CreateAssetMenu(fileName = "SpeakerObject", menuName = "Scriptable Objects/SpeakerObject")]
public class SpeakerObject : ScriptableObject
{
    public string displayName;
    public Sprite neutral;
    public Sprite angry;
    public Sprite happy;
    public Sprite sad;
    public Sprite surprised;

    private const string NEUTRAL_TAG = "neutral";
    private const string ANGRY_TAG = "angry";
    private const string HAPPY_TAG = "happy";
    private const string SAD_TAG = "sad";
    private const string SURPRISED_TAG = "surprised";

    public Sprite GetSprite(string name)
    {
        string value = name.ToLower();
        switch (name)
        {
            case NEUTRAL_TAG:
                return neutral;
            case ANGRY_TAG:
                return angry;
            case HAPPY_TAG:
                return happy;
            case SAD_TAG:
                return sad;
            case SURPRISED_TAG:
                return surprised;
            default:
                Debug.LogError("Sprite Name came in but is not currently being handle: " + name);
                break;
        }

        return null;
    }
}
