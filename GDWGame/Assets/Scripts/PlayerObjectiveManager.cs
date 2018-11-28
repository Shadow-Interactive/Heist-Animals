using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using SoundEngine;

public class PlayerObjectiveManager : NetworkBehaviour {
    
    public GameObject minigameCanvas;
    public RawImage[] theNumbers;
    public Text timerText;

    int attemptCounter = 0, attemptNum = 1, normalAttempt = 1, trapAttempt = 2, currentCode = 0;
    float mNormalCount = 30, mCounter = 0, counterLimit = 30, mTrapCount = 15;
    int[] playerCodes = new int[4];
    string NoDecimals = "F0", runner0ne = "RunnerOne", runnerTwo = "RunnerTwo";

    [HideInInspector] public PlayerLogic thePlayer;
    [HideInInspector] public bool smoke = false; //this is bad bleh

    // Use this for initialization
    void Start () {
        thePlayer = GetComponentInParent<PlayerLogic>();
        print(thePlayer);
	}

    void IncrementCounter()
    {
        theNumbers[currentCode].color = Color.red;
        if (currentCode < 3) currentCode++;
        theNumbers[currentCode].color = Color.green;
    }

    void DecrementCounter()
    {
        theNumbers[currentCode].color = Color.red;
        if (currentCode > 0) currentCode--;
        theNumbers[currentCode].color = Color.green;
    }

    public void InputNumber(int num)
    {
        print("Pls :(");
        playerCodes[currentCode] = num;
        theNumbers[currentCode].GetComponentInChildren<Text>().text = num.ToString();
        IncrementCounter();
    }

    public void GoBack()
    {
        DecrementCounter();
    }

    public void GoForward()
    {
        IncrementCounter();
    }

    public void ClearAll()
    {
        currentCode = 0;
        for (int i = 0; i < 4; i++)
        {
            playerCodes[i] = 0;
            theNumbers[i].GetComponentInChildren<Text>().text = 0.ToString();
            theNumbers[i].color = Color.red;
        }
        theNumbers[0].color = Color.green;
    }

    public void ActivateMinigame()
    {
        minigameCanvas.SetActive(true);
        mCounter = 0;
        thePlayer.SetMovement(true);
        thePlayer.setCursor(false);
        //thePlayer.SetActivation(true);
        StartCoroutine(MiniGame());
    }

    public void DeactivateMinigame()
    {
        ClearAll();
        minigameCanvas.SetActive(false);
        ResetMinigameNumbers();
        thePlayer.setCursor(true);
        thePlayer.SetMovement(false);
        thePlayer.SetActivation(false);
        if (thePlayer.name == runner0ne)
            thePlayer.R1currentObjective.DecoupleTrap();
        if (thePlayer.name == runnerTwo)
            thePlayer.R2currentObjective.DecoupleTrap();
        smoke = false;
    }

    void ResetMinigameNumbers()
    {
        attemptCounter = 0;
        attemptNum = normalAttempt;
        counterLimit = mNormalCount;
        mCounter = 0;
        timerText.text = 0.ToString();
    }

    public void Confirm()
    {
        if (thePlayer.name == runner0ne)
        {
            if (!CheckCode(thePlayer.R1currentObjective))
            {
                attemptCounter++;

                //player's code entry was successful
                if (attemptCounter >= attemptNum)
                {
                    print("Hey there");
                    attemptCounter = 0;
                    DeactivateMinigame();
                    StopCoroutine(MiniGame());
                    //thePlayer.currentObjective.DeActivate());
                    thePlayer.CmdDeactivateTrap(thePlayer.R1currentObjective.name);
                   
                    //thePlayer.currentObjective.DeActivate();
                    //thePlayer.R1currentObjective.GameObjectVisible(thePlayer.R1currentObjective.trapActive);

                    SoundManager.setPlaying(true, 14);
                    SoundManager.setLoop(14, false);

                    SoundManager.setVelocity(0f, 0f, 0f, 14);

                    Vector3 pos = thePlayer.GetComponent<Transform>().position;
                    SoundManager.setPosition(pos.x, pos.y, pos.z, 14);

                    SoundManager.setVolume(20f, 14);

                    SoundManager.playSound(14, Time.deltaTime);
                }
                //code entry was unsuccessful
                else
                {
                    print("killme <3 ");
                    thePlayer.Reshuffle();
                    ClearAll();


                }
            }
            else
            {
                SoundManager.setPlaying(true, 15);
                SoundManager.setLoop(15, false);

                SoundManager.setVelocity(0f, 0f, 0f, 15);

                Vector3 pos = thePlayer.GetComponent<Transform>().position;
                SoundManager.setPosition(pos.x, pos.y, pos.z, 15);

                SoundManager.setVolume(17f, 15);

                SoundManager.playSound(15, Time.deltaTime);
            }
        }
        else if (thePlayer.name == runnerTwo)
        {
            if (!CheckCode(thePlayer.R2currentObjective))
            {
                attemptCounter++;

                if (attemptCounter >= attemptNum)
                {
                    print("HeyJude");
                    attemptCounter = 0;
                    StopCoroutine(MiniGame());
                    DeactivateMinigame();
                    thePlayer.CmdDeactivateTrap(thePlayer.R2currentObjective.name);
                    
                }
                else
                {
                    print("HAIRESHUFFLE");
                    
                    thePlayer.Reshuffle();
                    ClearAll();
                }
            }
        }
    }

    bool CheckCode(Objective currentObjective)
    {
        for (int i = 0; i < 4; i++)
        {
            if (currentObjective.trapCode[i] != playerCodes[i])
            {
                return true;
            }
        }
        return false;
       
    }

    IEnumerator MiniGame()
    {
        thePlayer.SetActivation(true);
        mCounter = 0;
        float hackyTimer = 0;

        while (thePlayer.GetActivation())
        {
            mCounter += Time.deltaTime;
            hackyTimer += Time.deltaTime;
            SetTimerText(mCounter);

            if (smoke && hackyTimer >= 7)
            {
                thePlayer.Reshuffle();
                hackyTimer = 0;
            }

            if (mCounter > counterLimit)
            {
                DeactivateMinigame();
                thePlayer.TrapFailure();
                hackyTimer = 0;
                StopCoroutine(MiniGame());
                yield break;
            }
            yield return null;
        }
    }

    void SetTimerText(float val)
    {
        timerText.text = val.ToString(NoDecimals);
    }

    public void WiredCaseTrap()
    {
        counterLimit = mTrapCount;
    }

    public void ReinforcedCaseTrap()
    {
        attemptNum = trapAttempt;
    }

    public void SmokeBombTrap()
    {
        smoke = true;
    }
}
