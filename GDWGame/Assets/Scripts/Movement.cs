using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

using System.Runtime.InteropServices;

public class Movement : NetworkBehaviour {

    //constraints for the camera y rotation
    private const float CAM_Y_MIN = -80.0f;
    private const float CAM_Y_MAX = 80.0f;
    //player speed per second (expected 60 FPS)
    public float speed = 600.0f; //speed without time.Deltatime was 10

    //camera variables for tracking camera position, target, and camera object itself
    public Transform cameraTarget;
    public Transform playerCam;
    public Transform noDownWardForce;
    public Camera viewCam;
    public Camera NoDownWardForceCam;

    //private variables to gather mouse and keyboard input
    private Vector2 moveInput;
    private Vector2 cameraLook;

    private Vector3 ray;
    private bool mouseLock = true;
    public bool spawnIn = false;

    [HideInInspector] public bool disableMovement = false;

    const string dllName = "FMOD Controller";

    [DllImport(dllName)]
    public static extern void setListenerPos(float _x, float _y, float _z);

    [DllImport(dllName)]
    public static extern void setListenerVel(float _x, float _y, float _z);

    [DllImport(dllName)]
    public static extern void setListenerUp(float _x, float _y, float _z);

    [DllImport(dllName)]
    public static extern void setListenerForward(float _x, float _y, float _z);

    // Use this for initialization
    void Start () {

        //lock cursor to the game window, specifically the center of the window
        Cursor.lockState = CursorLockMode.Locked;
        //setup players for local and connected clients
        if (isLocalPlayer)
        {
            if (isServer)
            {
               
                gameObject.transform.position = new Vector3(0f, 0f, 0f);
                gameObject.tag = "RunnerOne";

            }
            else
            {
                
                gameObject.transform.position = new Vector3(2f, 1f, 0f);
                gameObject.tag = "RunnerTwo";
                
            }
        }
        else
        {
            if (isServer)
            {
                gameObject.transform.position = new Vector3(2f, 1f, 0f);
                gameObject.tag = "RunnerTwo";
            }
            else
            {
                gameObject.transform.position = new Vector3(0f, 0f, 0f);
                gameObject.name = "RunnerOne";
            }
        }

    }
	// Update is called once per frame
	void Update () {

        
        //only update the local player, otherwise exit
        if (!isLocalPlayer)
        {
            //don't want to override previous player cameras
            viewCam.enabled = false;
            NoDownWardForceCam.enabled = false;
            return;
        }

        //fill moveInput with keyboard input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput.Normalize();

        //vector containing moveInput
        Vector3 playerControl = new Vector3(moveInput.x, 0, moveInput.y);
        
        //fill cameraLook with mouse input
        cameraLook.x += Input.GetAxis("Mouse X");
        cameraLook.y += Input.GetAxis("Mouse Y");

        //clamp the camera's y rotation to prevent weird camera angles
        cameraLook.y = Mathf.Clamp(cameraLook.y, CAM_Y_MIN, CAM_Y_MAX);

        //create the rotation for camera
        Quaternion cameraRotate = Quaternion.Euler(-cameraLook.y, cameraLook.x, 0);
        //create the rotation for the Y rotation only camera
        Quaternion cameraRotateOnlyY = Quaternion.Euler(0, cameraLook.x, 0);
        //update the position of the camera, using the player's position and camera rotation, alongside the distance from the camera to player
        playerCam.position = cameraTarget.position + cameraRotate * new Vector3(1,0,-2);
        //want the camera always looking at the player, since it's a 3rd person camera
        playerCam.LookAt(cameraTarget);

        //setup the Y rotation only camera to properly get line of sight for character movement and velocity
        noDownWardForce.position = new Vector3(playerCam.position.x, cameraTarget.position.y, playerCam.position.z) + cameraRotateOnlyY * new Vector3(0, 0 ,0);
        noDownWardForce.LookAt(cameraTarget);

        
        //want the player to move in the forward direction of the Y rotation onlys camera (to avoid any downward force when camera is tilted), rather than just on it's local axis
        Vector3 moveInDirectionOfCam = NoDownWardForceCam.transform.TransformVector(playerControl);
        //gameObject.transform.Rotate(new Vector3(0, moveInDirectionOfCam.x, 0));
        //y isn't needed for movement
        moveInDirectionOfCam.y = 0;

        if (!disableMovement)
            //set the velocity of the rigidbody
            GetComponent<Rigidbody>().velocity = moveInDirectionOfCam * speed * Time.deltaTime;
        //print(GetComponent<Rigidbody>().velocity);

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            mouseLock = !mouseLock;
        }

        if (mouseLock)
            Cursor.lockState = CursorLockMode.Locked;
        else if (!mouseLock)
            Cursor.lockState = CursorLockMode.None;
        //create a vector outwards from behind the camera
        Vector3 rayCastBehind = playerCam.transform.TransformDirection(Vector3.forward * -1);
        //variable for contact point of the raycast
        RaycastHit contact;
        //send out raycasts from behind the player to check if the ray is colliding with an object behind the player
        if (Physics.Raycast(cameraTarget.TransformPoint(new Vector3(0, 0, 0)), rayCastBehind, out contact, 2.0f) && contact.transform != playerCam)
        {
            
            //situate the camera at point of contact to avoid camera clipping into objects
            playerCam.position = new Vector3(contact.point.x, contact.point.y, contact.point.z);
            
        }
        
	}
    public void roleChose (Movement script)
    {
        script.spawnIn = !script.spawnIn;
        this.spawnIn = script.spawnIn;
        Debug.Log(spawnIn);
    }
}

    
