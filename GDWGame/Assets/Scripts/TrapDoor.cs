using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TrapDoor : NetworkBehaviour {
    
    private void OnMouseDown()
    {
        if (!gameObject.GetComponentInParent<RoomScript>().doorCooldown)
        {
            gameObject.GetComponentInParent<RoomScript>().TrapActivation();
        }
    }
}
