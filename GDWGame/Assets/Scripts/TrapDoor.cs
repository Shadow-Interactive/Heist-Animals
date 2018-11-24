﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TrapDoor : NetworkBehaviour {
    ParticleSystem theEm;

    private void Start()
    {
        theEm = GetComponent<ParticleSystem>();
    }

    public void OnSecurityClick()
    {
        if (!gameObject.GetComponentInParent<RoomScript>().doorCooldown)
        {
            GameObject.Find("TheCameraManager(Clone)").GetComponent<OverSeerControl>().CmdTrapActivate(GameObject.Find("TheCameraManager(Clone)").GetComponent<OverSeerControl>().camRoomName);
            //gameObject.GetComponentInParent<RoomScript>().TrapActivation();
            theEm.Play();

        }
    }
}
