using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour {

    public GameObject minigame;
    public int ScoreWorth = 1; //not properly in use
    bool regen = false;
    float regenTimer = 0, regenLimit = 0;
    bool ohgodhwy = false;

    public void TreasureOnClick(Texture[] theImages, OverSeerControl theOverseer)
    {
        minigame.GetComponent<Objective>().ActivateObject(theImages, theOverseer);//, theOverseer);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void SetScore(int newValue)
    {
        ScoreWorth = newValue;
    }

    public void RandomizeScore()
    {
        ScoreWorth = Random.Range(1, 5);
    }

    public void OnRunnerCollision(ref int runnerScore) //also not in use
    {
        runnerScore += ScoreWorth;
        regen = true;
        regenLimit = Random.Range(1, 3);
    }

    private void Update()
    {

        if (regen)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= regenLimit)
            {
                gameObject.SetActive(true);
                regenTimer = 0;
                regen = false;
            }
        }
    }
}
