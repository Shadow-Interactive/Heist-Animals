using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour {

    public int roomTag;

	// Use this for initialization
	void Start () {
		
	}

    public void uponEntering(ref int playerInt)
    {
        playerInt = roomTag;
        print("Player entered room #" + roomTag);
        //we can activate whatever traps or whatever else here
    }
}
