using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClipping : MonoBehaviour {

    float cameraDistance = 1.7f;
    float closestCamToPlayerPos = 0.25f;
    Vector3 camPos;
    public Vector3 originalPos;
    public Vector3 newPos;
    float clipPos;
    float offSet = 1;
    bool behind = false;
    bool wall = false;
    bool rightRay = false;
    bool leftRay = false;
    Vector3 ADS;
    Vector3 ADSnormalized;

    Vector3 noClipPositionBehind;
    Vector3 noClipPositionLeft;
    Vector3 noClipPositionRight;

	// Use this for initialization
	void Awake () {
        camPos = transform.localPosition.normalized;
        clipPos = transform.localPosition.magnitude;
        originalPos = transform.localPosition;

        wall = false;
        rightRay = false;
        leftRay = false;
        behind = false;

        ADS = transform.parent.localPosition + new Vector3(1.5f, -3f, 8f);
        ADSnormalized = ADS.normalized * -1;
        ADSnormalized += new Vector3(0.5f, 0, 0);
    }
	
	// Update is called once per frame
	void Update () {

        //destination our camera raycast will go
        //camPos = transform.localPosition.normalized;
        if (Input.GetKey(KeyCode.R))
        {
            cameraDistance = 1.3f;
            noClipPositionBehind = transform.parent.TransformPoint(new Vector3(ADSnormalized.x, ADSnormalized.y, ADSnormalized.z - 0.25f) * cameraDistance);
            noClipPositionLeft = transform.parent.TransformPoint(new Vector3(ADSnormalized.x - 0.2f, ADSnormalized.y, ADSnormalized.z - 0.15f) * cameraDistance);
            noClipPositionRight = transform.parent.TransformPoint(new Vector3(ADSnormalized.x + 0.2f, ADSnormalized.y, ADSnormalized.z - 0.15f) * cameraDistance);
        }
        else
        {
            cameraDistance = 1.7f;
            //three raycasts to check for collision
            noClipPositionBehind = transform.parent.TransformPoint(new Vector3(camPos.x, camPos.y, camPos.z - 0.25f) * cameraDistance);
            noClipPositionLeft = transform.parent.TransformPoint(new Vector3(camPos.x - 0.1f, camPos.y, camPos.z - 0.15f) * cameraDistance);
            noClipPositionRight = transform.parent.TransformPoint(new Vector3(camPos.x + 0.1f, camPos.y, camPos.z - 0.15f) * cameraDistance);
        }

        RaycastHit contact;

        
        //visual representation of the rays
        Debug.DrawLine(transform.parent.position, noClipPositionLeft, Color.red);
        Debug.DrawLine(transform.parent.position, noClipPositionBehind, Color.green);
        Debug.DrawLine(transform.parent.position, noClipPositionRight, Color.blue);

        //raycast behind
        if (Physics.Linecast(transform.parent.position, noClipPositionBehind, out contact) && wall == false)
        {
            behind = true;
            transform.position = Vector3.Lerp(transform.position, new Vector3(contact.point.x + contact.normal.x * 0.3f, contact.point.y + contact.normal.y * 0.15f, contact.point.z + contact.normal.z * 0.3f), Time.deltaTime * 25.0f);
        }
        //raycast left diagonally
        else if (Physics.Linecast(transform.parent.position, noClipPositionLeft, out contact) && rightRay == false && behind == false)
        {
            leftRay = true;
            wall = true;
            //clipPos = Mathf.Clamp((contact.distance * 0.8f), closestCamToPlayerPos, cameraDistance);
            transform.position = Vector3.Lerp(transform.position, new Vector3(contact.point.x + contact.normal.x * 0.2f, contact.point.y + contact.normal.y * 0.15f, contact.point.z + contact.normal.z * 0.2f), Time.deltaTime * 25.0f);
        }
        //raycast right diagonally
        else if (Physics.Linecast(transform.parent.position, noClipPositionRight, out contact) && leftRay == false && behind == false)
        { 
            rightRay = true;
            wall = true;
            //clipPos = Mathf.Clamp((contact.distance * 0.8f), closestCamToPlayerPos, cameraDistance);
            transform.position = Vector3.Lerp(transform.position, new Vector3(contact.point.x + contact.normal.x * 0.3f, contact.point.y + contact.normal.y * 0.15f, contact.point.z + contact.normal.z * 0.3f), Time.deltaTime * 25.0f);
        }
        else
        {
            wall = false;
            leftRay = false;
            rightRay = false;
            behind = false;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * 15.0f);
            
            //transform.localPosition = originalPos;
            //clipPos = cameraDistance;
        }

        if (Input.GetKey(KeyCode.R))
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, ADS, Time.deltaTime * 2.0f);
        }
 

        //transform.localPosition = Vector3.Lerp(transform.localPosition, camPos, Time.deltaTime * 10);


        //transform.localPosition = Vector3.Lerp(new Vector3(offSet, transform.localPosition.y, transform.localPosition.z), camPos * clipPos, Time.deltaTime * 50);
    }
}
