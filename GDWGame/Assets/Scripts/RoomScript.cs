using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public int roomTag;
    public RoomTraps trapType;
    public RoomManager theManager;

    // Use this for initialization
    void Start()
    {

    }

    public bool uponEntering(ref int playerInt)
    {
        playerInt = roomTag;
        print("Player entered room #" + roomTag);
        //we can activate whatever traps or whatever else here

        if ((int)trapType == 2)
            return false;

        return true;

    }

    public void DeactivateTrap()
    {
        trapType = RoomTraps.NO_TRAP;
    }
}