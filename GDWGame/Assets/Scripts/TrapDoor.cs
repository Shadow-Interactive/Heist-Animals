using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TrapDoor : NetworkBehaviour {
    //ParticleSystem theEm;
    public ParticleSystem offLightning;
    public ParticleSystem offSparks; //the lighning and sparks for particles;

    private void Start()
    {
       // theEm = GetComponent<ParticleSystem>();
    }

    public void OnSecurityClick(GameObject activeOverseer)
    {
        if (!gameObject.GetComponentInParent<RoomScript>().doorCooldown)
        {
            activeOverseer.GetComponent<OverSeerControl>().CmdTrapActivate(activeOverseer.GetComponent<OverSeerControl>().camRoomName);

            //gameObject.GetComponentInParent<RoomScript>().CmdTrapActivation();
            //activeOverseer.GetComponent<OverSeerControl>().CmdTrapActivate(activeOverseer.GetComponent<OverSeerControl>().camRoomName, gameObject.GetComponentInParent<RoomScript>().trapActivated);
            //gameObject.GetComponentInParent<RoomScript>().TrapActivation();
            // theEm.Play();
            offLightning.Play(); //this plays the spark effects for the security boxes
            offSparks.Play();
            GetComponent<AudioSource>().Play(); //sound effects!!!

        }
    }
}
