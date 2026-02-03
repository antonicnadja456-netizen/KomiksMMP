using UnityEngine;
using UnityEngine.SceneManagement;

public class BGAudio : MonoBehaviour
{
    public float menuVolume = 0.2f;
    public float gameVolume = 0.1f;
    public AudioSource bgSource;

    private void Awake()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
    {
        if (arg1.name == "Menu")
        {
            bgSource.volume = menuVolume;
        }
        else
        {
            bgSource.volume = gameVolume;
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }
}
