using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    public LevelChanger fader;

    public void Select(int levelIndex)
    {
        fader.FadeToLevel(levelIndex);
    }
}
