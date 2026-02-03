using UnityEngine;

[CreateAssetMenu(fileName = "LevelObject", menuName = "Scriptable Objects/LevelObject")]
public class LevelObject : ScriptableObject
{
    public string Name;
    public string SceneName;
    public Sprite LevelSprite;
}