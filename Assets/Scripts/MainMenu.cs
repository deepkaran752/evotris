using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void BootAI() => SceneManager.LoadScene(1);

    public void ShutDownAI() => Application.Quit();
}
