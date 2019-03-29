using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HostButton : MonoBehaviour {

    private Button hostButton;
    private GameObject theLobbyManager;
    public GameObject hostDisconnect, joinButton, IPinput, title;

	// Use this for initialization
	void Start () {

        hostButton = gameObject.GetComponent<Button>();
        theLobbyManager = GameObject.Find("LobbyManager");

        hostButton.onClick.AddListener(hostGame);
	}
	
    void hostGame()
    {
        theLobbyManager.GetComponent<CustomSpawn>().ButtonHost();

        theLobbyManager.GetComponent<AudioSource>().Play();

        title.SetActive(false);
        gameObject.SetActive(false);
        joinButton.SetActive(false);
        IPinput.SetActive(false);
        hostDisconnect.SetActive(true); //activate host disconnect button
    }
	// Update is called once per frame
	void Update () {
        theLobbyManager = GameObject.Find("LobbyManager");
    }
}
