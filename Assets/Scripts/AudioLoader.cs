using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoader : MonoBehaviour
{
    // Start is called before the first frame update
    private static GameObject instance;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
            instance = gameObject;
        else
            Destroy(gameObject);

        if (SettingsController.muteMusic)
            MuteAudio();
    }

    public static void MuteAudio()
    {
        instance.GetComponent<AudioSource>().volume = 0;
    }

    public static void Blast()
    {
        instance.GetComponent<AudioSource>().volume = 1;
    }
}
