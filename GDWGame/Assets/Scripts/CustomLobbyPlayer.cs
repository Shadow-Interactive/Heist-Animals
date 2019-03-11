using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomLobbyPlayer : NetworkLobbyPlayer {

    private GameObject canvas;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        //canvas = GameObject.Find("ButtonHost");
        //canvas.SetActive(false); // change this later probably to make it easier to reactivate when lobby player is removed
	}

    public void ButtonStart()
    {
        base.SendReadyToBeginMessage();
    }

    //to be used for unreadying
    public void ButtonLeave()
    {
        base.SendNotReadyToBeginMessage();
    }
}
