using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlocker : MonoBehaviour
{

    public static void InitializeUnlocker()
    {
        // fix player prefs if corrupted
        if (!PlayerPrefs.HasKey("highestUnlockedLevel") || PlayerPrefs.GetInt("highestUnlockedLevel") <= 0) {
            PlayerPrefs.SetInt("highestUnlockedLevel", 1);
            PlayerPrefs.Save();
        }
    }

    public static void Unlocked()
    {
        PlayerPrefs.SetInt("highestUnlockedLevel", PlayerPrefs.GetInt("highestUnlockedLevel") + 1);
        PlayerPrefs.Save();
    }

    public static int GetHighestUnlockedLevel()
    {
        return PlayerPrefs.GetInt("highestUnlockedLevel");
    }
}
