using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : SharedInformation
{

    public RoomScript[] theRooms;

    //here we can do stuff like the player transporting to a random level, and anything else that would require access to the diff levels. 
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < theRooms.Length; i++)
        {
            theRooms[i].roomTag = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Teleport(ref Vector3 position, ref int roomInt)
    {
        int temp = Random.Range(0, theRooms.Length);
        position = theRooms[temp].transform.position;
        roomInt = temp;
        print("Player teleported to room #" + roomInt);

    }
}
