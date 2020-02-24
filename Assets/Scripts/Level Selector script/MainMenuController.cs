using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    public LevelChanger fader;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(GameObject.Find("funky tunes"));
    }

    public void Play()
    {
        fader.FadeToLevel("World_1");
    }

    public void Settings()
    {
        fader.FadeToLevel("Settings");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
