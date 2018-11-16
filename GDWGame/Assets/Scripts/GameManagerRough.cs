using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerRough : MonoBehaviour {

    //this will get moved
    double theTimerSeconds = 0;
    int theTimer = 0;
    bool gameOver = false;
    public GameObject gameoverScreen;
    public Text timerText;
    string NoDecimals = "F0";
    string withZero = ":0", withoutZero = ":";

    // Use this for initialization
    void Start () {
        theTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (theTimerSeconds % 60 >= 59.7)
        {
            theTimer++;
            theTimerSeconds = 0;
        }

        if (theTimerSeconds < 9)
            timerText.text = theTimer.ToString(NoDecimals) + withZero + theTimerSeconds.ToString(NoDecimals);
        else
            timerText.text = theTimer.ToString(NoDecimals) + withoutZero + theTimerSeconds.ToString(NoDecimals);


        if (theTimer >= 10)
        {
            gameoverScreen.SetActive(true);
        }

        theTimerSeconds += 1 * Time.deltaTime;
        
	}
}
