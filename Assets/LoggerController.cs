using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using cse481.logging;


public class LoggerController : MonoBehaviour
{
    public static CapstoneLogger LOGGER;
    // Start is called before the first frame update

    public static int numRestarts;
    public static int deathCount;

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

        numRestarts = 0;
        deathCount = 0;

        string skey = "1f16a6c017f12c1539c4947e234de78a";
        int gameId = 202009;
        string gameName = "jayswalkin";
        int cid = -5;
        // -1... -inf also debug / maintenance
        // MOST RECENT: -5 ********** REMEMBER TO UPDATE **************
        // 0 Debug and Maintenance
        // 1 First test run
        // 2 Itch.IO version
        // 3 Newgrounds version


        int userId = PlayerPrefs.GetInt("playerid", -1);

        if (userId == -1)
        {
            userId = Random.Range(int.MinValue, int.MaxValue);
            PlayerPrefs.SetInt("playerid", userId);
        }

        CapstoneLogger logger = new CapstoneLogger(gameId, gameName, skey, cid);

        StartCoroutine(logger.StartNewSession("" + userId));
        LoggerController.LOGGER = logger;

    }

    void Start()
    {
    }

    // Update is called once per frame
    public static void ResetFields()
    {
        numRestarts = 0;
        deathCount = 0;
    }

    public void Play()
    {
        //LoggerController.LOGGER.LogActionWithNoLevel(199, "press play");

    }
}
