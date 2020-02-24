using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    public LevelChanger fader;

    public void Play()
    {
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
