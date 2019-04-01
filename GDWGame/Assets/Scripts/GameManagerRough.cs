﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class GameManagerRough : NetworkBehaviour {

    //this will get moved
    double theTimerSeconds = 60;
    int theTimer = 4;
    bool gameOver = false;
    public GameObject gameoverScreen;
    public Text timerText, hackyOverseerText;
    string NoDecimals = "F0", withZero = ":0", withoutZero = ":", UserTeamWins = "Your team won!", EnemyTeamWins = "Enemy team won...", Tie = "It's a tie!";
    PlayerLogic RunnerOne, RunnerTwo;
    OverSeerControl OverseerOne, OverseerTwo;
    public Text Score1, Score2, WinningText;
    int player1Score = 0;
    int player2Score = 0; //this specifically is to make it easier to test

    int gameTimeLimit = -1;
    bool theGameOver = false;
    bool initGM = true;

    // Use this for initialization
    void Start () {
        theTimer = 4;
	}

    // Update is called once per frame
    void Update()
    {
        if (initGM)
        {
            LoadProperties();
        }
        
        if (!theGameOver)
        { 


            if (theTimerSeconds <= 0)
            {
                theTimer--;
                theTimerSeconds = 60;
            }

            if (theTimerSeconds < 9)
            {
                hackyOverseerText.text = theTimer.ToString(NoDecimals) + withZero + theTimerSeconds.ToString(NoDecimals);
                timerText.text = theTimer.ToString(NoDecimals) + withZero + theTimerSeconds.ToString(NoDecimals);
            }
            else
            {
                hackyOverseerText.text = theTimer.ToString(NoDecimals) + withoutZero + theTimerSeconds.ToString(NoDecimals);
                timerText.text = theTimer.ToString(NoDecimals) + withoutZero + theTimerSeconds.ToString(NoDecimals);
            }

            theTimerSeconds -= 1 * Time.deltaTime;

            if (theTimer <= gameTimeLimit)
            {
                theGameOver = true;
                GameOver();
            }
            if (theTimerSeconds <= 0)
            {
                theTimer--;
                theTimerSeconds = 60;
            }
        }

    }

    public void SetRunnerUIActive(bool active)
    {
        timerText.gameObject.SetActive(active);
    }

    public void SetOverseerUIActive(bool active)
    {
        hackyOverseerText.gameObject.SetActive(active);
    }

    public void GameOver()
    {
        gameoverScreen.SetActive(true);
        

        //   if (isServer)
        //{
        if (RunnerOne != null)
            {
            if (RunnerOne.team == 0)
                player1Score = RunnerOne.numTreasures;
            else if (RunnerOne.team == 2)
                player1Score = RunnerOne.numTreasures;
            }

            if (RunnerTwo != null)
            {
            if (RunnerTwo.team == 0)
                player2Score = RunnerTwo.numTreasures;
            else if (RunnerTwo.team == 2)
                player2Score = RunnerTwo.numTreasures;
        }
     //   }

        Score1.text = player1Score.ToString(NoDecimals);
        Score2.text = player2Score.ToString(NoDecimals);

        if (player1Score > player2Score)
        {
            if (RunnerOne.isLocalPlayer || OverseerOne.isLocalPlayer)
                WinningText.text = UserTeamWins;
            else if (RunnerTwo.isLocalPlayer || OverseerTwo.isLocalPlayer)
                WinningText.text = EnemyTeamWins;
        }
        else if (player1Score < player2Score)
        {
            if (RunnerTwo.isLocalPlayer || OverseerTwo.isLocalPlayer)
                WinningText.text = UserTeamWins;
            else if (RunnerOne.isLocalPlayer || OverseerOne.isLocalPlayer)
                WinningText.text = EnemyTeamWins;
        }
        else
            WinningText.text = Tie;
    }

    void LoadProperties()
    {
        try
        {
            if (GameObject.FindGameObjectsWithTag("Runner")[0].GetComponent<PlayerLogic>().team == 0)
                RunnerOne = GameObject.FindGameObjectsWithTag("Runner")[0].GetComponent<PlayerLogic>();
            else if (GameObject.FindGameObjectsWithTag("Runner")[1].GetComponent<PlayerLogic>().team == 0)
                RunnerOne = GameObject.FindGameObjectsWithTag("Runner")[1].GetComponent<PlayerLogic>();
            //print("ffs");

            if (GameObject.FindGameObjectsWithTag("Runner")[1].GetComponent<PlayerLogic>().team == 2)
                RunnerTwo = GameObject.FindGameObjectsWithTag("Runner")[1].GetComponent<PlayerLogic>();
            else if (GameObject.FindGameObjectsWithTag("Runner")[0].GetComponent<PlayerLogic>().team == 2)
                RunnerTwo = GameObject.FindGameObjectsWithTag("Runner")[0].GetComponent<PlayerLogic>();
            //print("ffs2");

            if (GameObject.FindGameObjectsWithTag("Overseer")[0].GetComponent<OverSeerControl>().team == 0)
                OverseerOne = GameObject.FindGameObjectsWithTag("Overseer")[0].GetComponent<OverSeerControl>();
            else if (GameObject.FindGameObjectsWithTag("Overseer")[1].GetComponent<OverSeerControl>().team == 0)
                OverseerOne = GameObject.FindGameObjectsWithTag("Overseer")[1].GetComponent<OverSeerControl>();

            if (GameObject.FindGameObjectsWithTag("Overseer")[1].GetComponent<OverSeerControl>().team == 2)
                OverseerTwo = GameObject.FindGameObjectsWithTag("Overseer")[1].GetComponent<OverSeerControl>();
            else if (GameObject.FindGameObjectsWithTag("Overseer")[0].GetComponent<OverSeerControl>().team == 2)
                OverseerTwo = GameObject.FindGameObjectsWithTag("Overseer")[0].GetComponent<OverSeerControl>();
        }
        catch {
            print("Some players missing");
        }
        //print("ffs");
        if (RunnerOne != null && RunnerOne.isLocalPlayer)
        {
            hackyOverseerText.gameObject.SetActive(false);
        }
        if (RunnerTwo != null && RunnerTwo.isLocalPlayer)
        {
            hackyOverseerText.gameObject.SetActive(false);

        }

        if (OverseerOne != null && OverseerOne.isLocalPlayer)
        {
            timerText.gameObject.SetActive(false);
        }
        if (OverseerTwo != null && OverseerTwo.isLocalPlayer)
        {
            timerText.gameObject.SetActive(false);
        }

        initGM = false;
    }
}
