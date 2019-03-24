using System.Collections;
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
    public Text timerText;
    string NoDecimals = "F0", withZero = ":0", withoutZero = ":", Team1Wins = "Team 1 Wins!", Team2Wins = "Team 2 Wins!", Tie = "It's a tie!";
    PlayerLogic RunnerOne, RunnerTwo;
    public Text Score1, Score2, WinningText;
    int player1Score = 0;
    int player2Score = 0; //this specifically is to make it easier to test

    int gameTimeLimit = -1;
    bool theGameOver = false;

    // Use this for initialization
    void Start () {
        theTimer = 4;
	}

    // Update is called once per frame
    void Update()
    {
        

        if (!theGameOver)
        { 
            if (theTimerSeconds <= 0)
            {
                theTimer--;
                theTimerSeconds = 60;
            }

            if (theTimerSeconds < 9)
                timerText.text = theTimer.ToString(NoDecimals) + withZero + theTimerSeconds.ToString(NoDecimals);
            else
            timerText.text = theTimer.ToString(NoDecimals) + withoutZero + theTimerSeconds.ToString(NoDecimals);

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

    public void GameOver()
    {
        gameoverScreen.SetActive(true);

        if (GameObject.FindGameObjectsWithTag("Runner")[0])
            RunnerOne = GameObject.FindGameObjectsWithTag("Runner")[0].GetComponent<PlayerLogic>();
        else
            print("ffs");

        if (GameObject.FindGameObjectsWithTag("Runner")[1])
            RunnerTwo = GameObject.FindGameObjectsWithTag("Runner")[1].GetComponent<PlayerLogic>();
        else
            print("ffs2");

        //   if (isServer)
        //{
        if (RunnerOne != null)
            {
            if (RunnerOne.team == 0)
                player1Score = RunnerOne.numTreasures;
            else if (RunnerOne.team == 2)
                player1Score = RunnerTwo.numTreasures;
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
            WinningText.text = Team1Wins;
        else if (player1Score < player2Score)
            WinningText.text = Team2Wins;
        else
            WinningText.text = Tie;
    }
}
