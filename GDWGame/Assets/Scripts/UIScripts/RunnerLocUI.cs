using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunnerLocUI : MonoBehaviour {

    //variables
    Vector3[] runnerLocPositions = new Vector3[14];
    public RawImage uiElement;

    // Use this for initialization
    void Start () {
        //setting the runner positions
        //also known as hell
        runnerLocPositions[0] = new Vector3(-24.3f, 19.8f, 0); //c
        runnerLocPositions[1] = new Vector3(-9.1f, 21.2f, 0); //c
        runnerLocPositions[2] = new Vector3(0.8f, 21.2f, 0); //c
        runnerLocPositions[3] = new Vector3(12.9f, 21.2f, 0); //c
        runnerLocPositions[4] = new Vector3(26.6f, 21.2f, 0); //c
        runnerLocPositions[5] = new Vector3(20.2f, -3.1f, 0);//c
        runnerLocPositions[6] = new Vector3(19, -25.8f, 0); //c
        runnerLocPositions[7] = new Vector3(2, -25.8f, 0); //c
        runnerLocPositions[8] = new Vector3(-9.9f, -25.8f, 0); //c
        runnerLocPositions[9] = new Vector3(-19.8f, -39, 0); //c
        runnerLocPositions[10] = new Vector3(-31.4f, -24.3f, 0); //c
        runnerLocPositions[11] = new Vector3(-31.4f, -9.1f, 0);
        runnerLocPositions[12] = new Vector3(-27.1f, 1.1f, 0); // c
        runnerLocPositions[13] = new Vector3(-3.1f, -5.3f, 0); // c

    }

    // Update is called once per frame
    void Update () {
		
	}

    public void SetPosition(int index)
    {
        //sets corresponding map position
        //roomint matches the position ints

        uiElement.GetComponent<RectTransform>().anchoredPosition = runnerLocPositions[index];
    }
}
