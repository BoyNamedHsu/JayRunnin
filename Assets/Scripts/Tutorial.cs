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
        if (LevelSelector.levelChosen == 1)
            GameObject.Instantiate(crossRoadTutorial);
        else if (LevelSelector.levelChosen == 4)
            GameObject.Instantiate(killCopsTutorial);
        else if (LevelSelector.levelChosen == 3)
            GameObject.Instantiate(rToRestartTutorial);
        else if (LevelSelector.levelChosen == 5)
            GameObject.Instantiate(saveFollowersTutorial);
        else if (LevelSelector.levelChosen == 10)
            GameObject.Instantiate(finalText);

    }
}
