using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void BootAI()
    {
        AudioManager.Instance?.PlayButtonClickSound();
        SceneManager.LoadScene(1);
    }

    public void ShutDownAI()
    {
        AudioManager.Instance?.PlayButtonClickSound();
        Application.Quit();
    }
}
