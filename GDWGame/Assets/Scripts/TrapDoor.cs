using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TrapDoor : NetworkBehaviour {
    
    public void OnSecurityClick()
    {
        if (!gameObject.GetComponentInParent<RoomScript>().doorCooldown)
        {
            gameObject.GetComponentInParent<RoomScript>().TrapActivation();
        }
    }
}
