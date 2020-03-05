using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlocker : MonoBehaviour
{
    private static int highest; // highest level unlocked

    // Start is called before the first frame update
    void Start()
    {
        highest = PlayerPrefs.HasKey("highestUnlockedLevel") ? PlayerPrefs.GetInt("highestUnlockedLevel") : 1;
    }

    public static void Unlocked()
    {
        highest++;
        PlayerPrefs.SetInt("highestUnlockedLevel", highest);
        PlayerPrefs.Save();
    }

    public static int GetHighestUnlockedLevel()
    {
        return highest;
    }
}
