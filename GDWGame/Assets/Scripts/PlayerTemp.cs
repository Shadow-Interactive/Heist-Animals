using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTemp : MonoBehaviour {

    int playerInt;
    Vector3 moveUp, moveDown, moveRight, moveLeft, playerPos;
    float speed = 5f;
    string doorStr = "Door";

    public RoomManager theRoomManager;

    // Use this for initialization
    void Start () {
        moveUp.Set(-speed, 0, 0);
        moveDown.Set(speed, 0, 0);
        moveRight.Set(0, 0, speed);
        moveLeft.Set(0, 0, -speed);
        playerInt = 8;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(moveLeft * Time.deltaTime, Space.World);

        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(moveDown * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(moveUp * Time.deltaTime, Space.World);

        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(moveRight * Time.deltaTime, Space.World);

        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector3 position = transform.position;
            theRoomManager.Teleport(ref position, ref playerInt);
            transform.position = new Vector3(position.x, 0.073f, position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(doorStr))
        {
           if ( playerInt != other.GetComponentInParent<RoomScript>().roomTag)
                other.GetComponentInParent<RoomScript>().uponEntering(ref playerInt);
        }
    }

}
