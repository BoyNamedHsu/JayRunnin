using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public LevelChanger fader;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Play()
    {
        fader.FadeToLevel(1);
        Debug.Log('e');
    }

    public void Settings()
    {
        fader.FadeToLevel(2);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
