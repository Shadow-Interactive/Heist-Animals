using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomLobbyPlayer : NetworkLobbyPlayer {

    private GameObject canvas;
    public GameObject readyButton, readyImage;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        readyImage.gameObject.SetActive(false);

        //canvas = GameObject.Find("ButtonHost");
        //canvas.SetActive(false); // change this later probably to make it easier to reactivate when lobby player is removed
    }

    public void ButtonStart()
    {
        base.SendReadyToBeginMessage();
        readyButton.gameObject.SetActive(false);
        readyImage.gameObject.SetActive(true);
    }

    //to be used for unreadying
    public void ButtonLeave()
    {
        if (base.readyToBegin)
        {
            base.SendNotReadyToBeginMessage();
            readyImage.gameObject.SetActive(false);
        }
    }
}
