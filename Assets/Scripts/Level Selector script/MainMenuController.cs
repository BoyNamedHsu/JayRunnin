using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using cse481.logging;

public class MainMenuController : MonoBehaviour
{
    public CapstoneLogger logger;
    public LevelChanger fader;

    public void Awake()
    {
        Unlocker.InitializeUnlocker();
        logger = LoggerController.LOGGER;
        StartCoroutine(logger.LogLevelStart(0, ""));
    }

    public void Play()
    {
        logger.LogLevelEnd(""); // Log end of level
        fader.FadeToLevel("World_1");
    }

    public void Settings()
    {
        fader.FadeToLevel("Settings");
    }
    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Mute()
    {
        AudioListener.volume = AudioListener.volume > 0 ? 0 : 1;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
