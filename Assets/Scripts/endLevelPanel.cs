﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class endLevelPanel : MonoBehaviour
{
    public GameObject levelCompleteUI;
    public GameObject[] stars;
    public Animator animator;
    public TMP_Text followersLeftText;
    //public GameObject levels;
    // Start is called before the first frame update
    void Start()
    {
        levelCompleteUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activatePanel()
    {
        animator.Play("endLevelPanel");
    }

    // Calculates how many stars player earned and shows the level complete panel 
    public void showPanel(int three, int two, int numFollowers, int followersDead, int followersUp)
    {
        int followersLeft = followersUp;
        
        levelCompleteUI.SetActive(true);
        Debug.Log("set active");
        followersLeftText.text = "" + (numFollowers - followersUp);
        int totalStars;

        if (followersLeft < three && followersLeft >= two)
        {
            totalStars = 2;
        }
        else if (followersLeft < two)
        {
            totalStars = 1;
        }
        else
        {
            totalStars = 3;
        }
        Debug.Log(followersLeft);
        for (int i = 0; i < totalStars; i++)
        {
            stars[i].SetActive(true);
        }

        activatePanel();
    }
}
