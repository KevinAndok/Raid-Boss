using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController instance;

    public AudioSource audioSource;

    private void Awake()
    {
        if (!instance) instance = this;
    }

    private void OnDisable()
    {
        instance = null;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void GoToURL(string url)
    {
        if (url != "") Application.OpenURL(url);
    }

    public void StartButton()
    {
        if (PartyCreation.roles.Count > 0) 
        {
            SceneManager.LoadScene(1);
        }
    }
}
