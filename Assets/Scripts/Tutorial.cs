using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject crossRoadTutorial, saveFollowersTutorial, 
                    killCopsTutorial, rToRestartTutorial, finalText;

    // Start is called before the first frame update
    void Start()
    {
        GameObject canvas = GameObject.Find("CanvasUI");

        if (LevelSelector.levelChosen == 1)
        {
            var tutorial = GameObject.Instantiate(crossRoadTutorial);
            tutorial.transform.SetParent(canvas.transform);
        }
        else if (LevelSelector.levelChosen == 3)
        {
            var tutorial = GameObject.Instantiate(killCopsTutorial);
            tutorial.transform.SetParent(canvas.transform);
        }
        else if (LevelSelector.levelChosen == 2)
        {
            var tutorial = GameObject.Instantiate(saveFollowersTutorial);
            tutorial.transform.SetParent(canvas.transform);
        }

    }
}
