﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisconnectButton : MonoBehaviour
{

    private Button disconnectButton;
    private GameObject theLobbyManager;
    public GameObject JoinButton, HostButton, IPinput, title;

    // Use this for initialization
    void Start()
    {

        disconnectButton = gameObject.GetComponent<Button>();
        theLobbyManager = GameObject.Find("LobbyManager");

        disconnectButton.onClick.AddListener(hostGame);
        gameObject.SetActive(false);
    }

    void hostGame()
    {
        theLobbyManager.GetComponent<CustomSpawn>().ButtonDisconnect();
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