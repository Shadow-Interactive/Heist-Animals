using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TrapDoor : NetworkBehaviour {
    ParticleSystem theEm;

    float pitchAdjust;

    private void Start()
    {
        theEm = GetComponent<ParticleSystem>();
        pitchAdjust = 0.1f;
    }

    public void OnSecurityClick(GameObject activeOverseer)
    {
        if (!gameObject.GetComponentInParent<RoomScript>().doorCooldown)
        {
            pitchAdjust *= -1;

            activeOverseer.GetComponent<OverSeerControl>().CmdTrapActivate(activeOverseer.GetComponent<OverSeerControl>().camRoomName);

            GetComponent<AudioSource>().pitch += pitchAdjust;

            //gameObject.GetComponentInParent<RoomScript>().CmdTrapActivation();
            //activeOverseer.GetComponent<OverSeerControl>().CmdTrapActivate(activeOverseer.GetComponent<OverSeerControl>().camRoomName, gameObject.GetComponentInParent<RoomScript>().trapActivated);
            //gameObject.GetComponentInParent<RoomScript>().TrapActivation();
            theEm.Play();

        }
    }
}
