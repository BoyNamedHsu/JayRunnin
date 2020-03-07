using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource plop;
    
    public void PlaySound(string name)
    {
        if (name == "plop")
        {
            plop.Play();
        }
    }

}
