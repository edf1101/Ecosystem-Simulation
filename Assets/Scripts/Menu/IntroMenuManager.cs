using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroMenuManager : MonoBehaviour // this simple script manages the UI for the intro menu scene
{
    public void loadFull() // these get called on button presses ie for full load, quit button etc
    {
        PlayerPrefs.SetString("genMode", "Full");
        SceneManager.LoadSceneAsync("World");
    }
    public void loadQuick()
    {
        PlayerPrefs.SetString("genMode", "Quick");
        SceneManager.LoadSceneAsync("World");
    }
    public void loadNone()
    {
        PlayerPrefs.SetString("genMode", "None");
        SceneManager.LoadSceneAsync("World");
    }
    public void quit()
    {
        Application.Quit();
    }
}
