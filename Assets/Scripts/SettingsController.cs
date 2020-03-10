using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public static bool muteSFX = false;
    public static bool muteMusic = false;

    public Image musicIcon;
    public Image sfxIcon;

    public Sprite muted;
    public Sprite play;

    void Start()
    {
        SwitchSprite(sfxIcon, muteSFX);
        SwitchSprite(musicIcon, muteMusic);
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void MuteMusic()
    {
        muteMusic = !muteMusic;
        if (muteMusic)
            AudioLoader.MuteAudio();
        else AudioLoader.Blast();
        SwitchSprite(musicIcon, muteMusic);
    }

    public void MuteSFX()
    {
        muteSFX = !muteSFX;
        SwitchSprite(sfxIcon, muteSFX);
    }

    private void SwitchSprite(Image button, bool mute)
    {
        if (mute) button.sprite = muted;
        else button.sprite = play;
    }
}
