using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCutscene : MonoBehaviour
{
    public LevelChanger changer;

    void Awake()
    {
        Destroy(GameObject.Find("funky tunes"));
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("ChangeScene", 70f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            changer.FadeToLevel("MainMenu");
        }
    }

    private void ChangeScene()
    {
        changer.FadeToLevel("MainMenu");
    }
}
