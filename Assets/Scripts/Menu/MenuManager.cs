using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private string lastLevel = "";
    [SerializeField] private Button continueBtn;
    [SerializeField] private GameObject levelsCanvas;
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject creditsCanvas;

    private void Start()
    {
        Menu();
        lastLevel = PlayerPrefs.GetString("LastLevel", "");
        continueBtn.interactable = lastLevel != "";
    }

    public void Continue()
    {
        SceneManager.LoadScene(lastLevel);
    }

    public void Levels()
    {
        menuCanvas.SetActive(false);
        levelsCanvas.SetActive(true);
        creditsCanvas.SetActive(false);
    }

    public void Menu()
    {
        menuCanvas.SetActive(true);
        levelsCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
    }

    public void Credits()
    {
        menuCanvas.SetActive(false);
        levelsCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
