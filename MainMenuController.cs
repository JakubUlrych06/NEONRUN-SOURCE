using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("TUTORIAL");
    }

    public void LVL1()
    {
        SceneManager.LoadScene("LVL1");
    }    
    public void LVL2()
    {
        SceneManager.LoadScene("LVL2");
    }
    public void LVL3()
    {
        SceneManager.LoadScene("LVL3");
    }
    public void FREE()
    {
        SceneManager.LoadScene("TESTING");
    }


    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
