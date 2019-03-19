using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinButton : MonoBehaviour {

    private Button joinButton;
    private GameObject theLobbyManager;
    public GameObject clientDisconnect, HostButton, IPinput;

    // Use this for initialization
    void Start()
    {

        joinButton = gameObject.GetComponent<Button>();
        theLobbyManager = GameObject.Find("LobbyManager");

        joinButton.onClick.AddListener(hostGame);
    }

    void hostGame()
    {
        theLobbyManager.GetComponent<CustomSpawn>().ButtonJoin();

        theLobbyManager.GetComponent<AudioSource>().Play(); //plays menuClick.wav on click

        gameObject.SetActive(false);
        HostButton.SetActive(false);
        IPinput.SetActive(false);
        clientDisconnect.SetActive(true); //activate client disconnect button
    }
    // Update is called once per frame
    void Update()
    {
        theLobbyManager = GameObject.Find("LobbyManager");
    }
}
