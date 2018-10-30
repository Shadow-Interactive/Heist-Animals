using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObjectiveManager : MonoBehaviour {
    
    public GameObject minigameCanvas;
    public RawImage[] theNumbers;
    public Text timerText;

    int attemptCounter = 0, attemptNum = 1, normalAttempt = 1, trapAttempt = 2, currentCode = 0;
    float mNormalCount = 30, mCounter = 0, counterLimit = 30, mTrapCount = 15;
    int[] playerCodes = new int[4];

    PlayerLogic thePlayer;

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

    public void clearAll()
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
        StartCoroutine(MiniGame());
    }

    public void DeactivateMinigame()
    {
        clearAll();
        minigameCanvas.SetActive(false);
        attemptCounter = 0;
        attemptNum = normalAttempt;
        counterLimit = mNormalCount;
        mCounter = 0;
        timerText.text = 0.ToString();
        thePlayer.setCursor(true);
        thePlayer.SetMovement(false);

    }

    public void Confirm()
    {
        if (!checkCode(thePlayer.currentObjective))
        {
            attemptCounter++;

            if (attemptCounter >= attemptNum)
            {
                attemptCounter = 0;
                DeactivateMinigame();
                thePlayer.currentObjective.DeActivate();
            }
            else
            {
                thePlayer.currentObjective.Reshuffle();
                clearAll();
            }
        }
    }

    bool checkCode(Objective currentObjective)
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
            setTimerText(mCounter);

            if (mCounter > counterLimit)
            {
                DeactivateMinigame();
                StopCoroutine(MiniGame());
                yield break;
            }
            yield return null;
        }
    }

    void setTimerText(float val)
    {
        timerText.text = val.ToString();
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
