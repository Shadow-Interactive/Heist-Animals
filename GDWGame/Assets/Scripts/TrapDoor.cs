using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : MonoBehaviour {
    
    private void OnMouseDown()
    {
        if (!gameObject.GetComponentInParent<RoomScript>().doorCooldown)
        {
            gameObject.GetComponentInParent<RoomScript>().TrapActivation();
        }
    }
}
