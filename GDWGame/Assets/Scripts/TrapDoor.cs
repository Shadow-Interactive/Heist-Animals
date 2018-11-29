using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TrapDoor : NetworkBehaviour {
    ParticleSystem theEm;

    private void Start()
    {
        theEm = GetComponent<ParticleSystem>();
    }

    public void OnSecurityClick(GameObject activeOverseer)
    {
        if (!gameObject.GetComponentInParent<RoomScript>().doorCooldown)
        {
            activeOverseer.GetComponent<OverSeerControl>().CmdTrapActivate(activeOverseer.GetComponent<OverSeerControl>().camRoomName);

            //gameObject.GetComponentInParent<RoomScript>().CmdTrapActivation();
            //activeOverseer.GetComponent<OverSeerControl>().CmdTrapActivate(activeOverseer.GetComponent<OverSeerControl>().camRoomName, gameObject.GetComponentInParent<RoomScript>().trapActivated);
            //gameObject.GetComponentInParent<RoomScript>().TrapActivation();
            theEm.Play();

        }
    }
}
