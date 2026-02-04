using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelsManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private LevelObject[] levels;
    [SerializeField] private GameObject levelPrefab;

    void Awake()
    {
        foreach (var level in levels)
        {
            string sName = level.SceneName;
            GameObject obj = Instantiate(levelPrefab, spawnPoint, false);
            obj.GetComponentInChildren<Image>().sprite = level.LevelSprite;
            obj.GetComponentInChildren<TextMeshProUGUI>().text = level.Name;
            obj.GetComponent<Button>().onClick.AddListener(() => { PlayerPrefs.SetString("LastLevel", sName); SceneManager.LoadScene(sName); });
        }
    }
}
