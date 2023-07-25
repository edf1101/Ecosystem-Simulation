using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroMenuManager : MonoBehaviour
{
    public void loadFull()
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
