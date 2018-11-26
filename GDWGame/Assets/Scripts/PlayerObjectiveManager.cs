using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerObjectiveManager : NetworkBehaviour {
    
    public GameObject minigameCanvas;
    public RawImage[] theNumbers;
    public Text timerText;

    int attemptCounter = 0, attemptNum = 1, normalAttempt = 1, trapAttempt = 2, currentCode = 0;
    float mNormalCount = 30, mCounter = 0, counterLimit = 30, mTrapCount = 15;
    int[] playerCodes = new int[4];
    string NoDecimals = "F0";

    [HideInInspector] public PlayerLogic thePlayer;

    // Use this for initialization
    void Start () {
        thePlayer = GetComponentInParent<PlayerLogic>();
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
        if (thePlayer.name == "RunnerOne")
            thePlayer.R1currentObjective.DecoupleTrap();
        if (thePlayer.name == "RunnerTwo")
            thePlayer.R2currentObjective.DecoupleTrap();
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
        if (thePlayer.name == "RunnerOne")
        {
            if (!CheckCode(thePlayer.R1currentObjective))
            {
                attemptCounter++;

                if (attemptCounter >= attemptNum)
                {
                    print("Hey there");
                    attemptCounter = 0;
                    DeactivateMinigame();
                    //thePlayer.currentObjective.DeActivate());
                    thePlayer.CmdDeactivateTrap(thePlayer.R1currentObjective.name);
                    //thePlayer.currentObjective.DeActivate();
                    //thePlayer.R1currentObjective.GameObjectVisible(thePlayer.R1currentObjective.trapActive);
                }
                else
                {
                    thePlayer.Reshuffle();
                    ClearAll();
                }
            }
        }
        else if (thePlayer.name == "RunnerTwo")
        {
            if (!CheckCode(thePlayer.R2currentObjective))
            {
                attemptCounter++;

                if (attemptCounter >= attemptNum)
                {
                    print("HeyJude");
                    attemptCounter = 0;
                    DeactivateMinigame();
                    //thePlayer.currentObjective.DeActivate());
                    thePlayer.CmdDeactivateTrap(thePlayer.R2currentObjective.name);
                    //thePlayer.currentObjective.DeActivate();
                    //thePlayer.R1currentObjective.GameObjectVisible(thePlayer.R1currentObjective.trapActive);
                }
                else
                {
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

        while (thePlayer.GetActivation())
        {
            mCounter += Time.deltaTime;
            SetTimerText(mCounter);

            if (mCounter > counterLimit)
            {
                DeactivateMinigame();
                thePlayer.TrapFailure();
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
        thePlayer.Scramble();
    }
}
