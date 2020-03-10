using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource plop;
    public GameObject woosh;
    public AudioSource car;
    public AudioSource thud;
    public AudioSource close;

    private AudioSource[] wooshes;
    private int wooshIndex = 0;

    void Awake()
    {
        wooshes = woosh.GetComponents<AudioSource>();
    }

    public void PlaySound(string name)
    {
        if (!SettingsController.muteSFX)
        {
            if (name == "plop")
            {
                plop.Play();
            }
            else if (name == "woosh")
            {
                wooshes[wooshIndex].Play();
                wooshIndex = wooshIndex >= wooshes.Length - 1 ? 0 : wooshIndex + 1;
            }
            else if (name == "car")
            {
                car.Play();
            }
            else if (name == "thud")
            {
                thud.PlayOneShot(thud.clip);
            }
            else if (name == "close")
                close.Play();
        }
    }
}
