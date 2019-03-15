using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisconnectClientButton : MonoBehaviour {

    private Button disconnectClientButton;
    private GameObject theLobbyManager;
    public GameObject JoinButton, HostButton, IPinput, title;

    // Use this for initialization
    void Start()
    {
        
        disconnectClientButton = gameObject.GetComponent<Button>();
        theLobbyManager = GameObject.Find("LobbyManager");

        disconnectClientButton.onClick.AddListener(hostGame);
        gameObject.SetActive(false);
    }

    void hostGame()
    {
        theLobbyManager.GetComponent<CustomSpawn>().ButtonDisconnectClient();
        title.SetActive(true);
        JoinButton.SetActive(true);
        HostButton.SetActive(true);
        IPinput.SetActive(true);
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        theLobbyManager = GameObject.Find("LobbyManager");
    }
}
