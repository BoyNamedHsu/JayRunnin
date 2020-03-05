using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideIfLocked : MonoBehaviour
{
    public int levelNum;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().color = (levelNum <= Unlocker.GetHighestUnlockedLevel()) ?
            new Color32(238,238,238,255) : new Color32(238,238,238,50);
    }
}
