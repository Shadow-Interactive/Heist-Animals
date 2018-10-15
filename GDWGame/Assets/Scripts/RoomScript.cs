using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : SharedInformation
{
    public int roomTag;
    public TypesOfTraps trapType;
    public RoomManager theManager;

    // Use this for initialization
    void Start()
    {

    }

    public void uponEntering(ref int playerInt, ref bool shockTrap)
    {
        playerInt = roomTag;
        print("Player entered room #" + roomTag);
        //we can activate whatever traps or whatever else here

        if (trapType == TypesOfTraps.SHOCK)
        {
            shockTrap = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(strZap))
        {
            Destroy(other);
        }
    }
}