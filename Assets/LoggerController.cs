using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using cse481.logging;


public class LoggerController : MonoBehaviour
{
    public static CapstoneLogger LOGGER;
    // Start is called before the first frame update

    private static LoggerController loggerInstance;
    void Awake()
    {
        DontDestroyOnLoad(this);

        if (loggerInstance == null)
        {
            loggerInstance = this;
        }
        else
        {
            DestroyObject(gameObject);
        }
    }

    void Start()
    {

        string skey = "1f16a6c017f12c1539c4947e234de78a";
        int gameId = 202009;
        string gameName = "jayswalkin";
        int cid = 1;
        // 0 Debug and Maintenance
        // 1 First test run
        // 2 Second test run
        // n nth test run
        CapstoneLogger logger = new CapstoneLogger(gameId, gameName, skey, 0);


        string userId = logger.GetSavedUserId();
        Debug.Log(userId);
        if (userId == null)
        {
            userId = logger.GenerateUuid();
            logger.SetSavedUserId(userId);
        }

        Debug.Log(userId);
        StartCoroutine(logger.StartNewSession(userId));
        LoggerController.LOGGER = logger;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        //LoggerController.LOGGER.LogActionWithNoLevel(199, "press play");

    }
}
