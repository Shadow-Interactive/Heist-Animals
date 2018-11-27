using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class GameManagerRough : NetworkBehaviour {

    //this will get moved
    double theTimerSeconds = 0;
    int theTimer = 0;
    bool gameOver = false;
    public GameObject gameoverScreen;
    public Text timerText;
    string NoDecimals = "F0", withZero = ":0", withoutZero = ":", Team1Wins = "Team 1 Wins!", Team2Wins = "Team 2 Wins!", Tie = "It's a tie!";
    PlayerLogic RunnerOne, RunnerTwo;
    public Text Score1, Score2, WinningText;
    [SyncVar] int player1Score = 0;
    [SyncVar] int player2Score = 0; //this specifically is to make it easier to test

    bool loadProperties = false;
    int gameTimeLimit = 5;

    // Use this for initialization
    void Start () {
        theTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {

        if (!loadProperties)
        {
            if (GameObject.FindGameObjectWithTag("RunnerOne"))
                RunnerOne = GameObject.FindGameObjectWithTag("RunnerOne").GetComponent<PlayerLogic>();

            if (GameObject.FindGameObjectWithTag("RunnerTwo"))
                RunnerTwo = GameObject.FindGameObjectWithTag("RunnerTwo").GetComponent<PlayerLogic>();

            loadProperties = true;

        }

        if (theTimerSeconds % 60 >= 59.6)
        {
            theTimer++;
            theTimerSeconds = 0;
        }

        if (theTimerSeconds < 9)
            timerText.text = theTimer.ToString(NoDecimals) + withZero + theTimerSeconds.ToString(NoDecimals);
        else
            timerText.text = theTimer.ToString(NoDecimals) + withoutZero + theTimerSeconds.ToString(NoDecimals);


        if (theTimer >= gameTimeLimit)
        {
            GameOver();
        }

        theTimerSeconds += 1 * Time.deltaTime;
        
	}

    public void GameOver()
    {
        gameoverScreen.SetActive(true);

        if (RunnerOne != null)
        {
           // print("beepbooprunnerone");
            player1Score = RunnerOne.numTreasures;
        }

        if (RunnerTwo != null)
        {
           // print("beepbooprunner2");
            player2Score = RunnerTwo.numTreasures;
        }

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
