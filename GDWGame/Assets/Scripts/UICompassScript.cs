using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompassScript : MonoBehaviour {

    //used this as source
    // https://www.youtube.com/watch?v=8QdhlS11kd0

    private Vector3 compassDirection;
    public Transform thePlayer;

	// Use this for initialization
	void Start () {
        compassDirection = new Vector3(0, 0, 180);
	}
	
	// Update is called once per frame
	void Update () {
        //we only change one of the axis 
        compassDirection.z = thePlayer.eulerAngles.y + 180; //this is based off of where the runner is looking
        transform.localEulerAngles = compassDirection; //we need to set it like this
	}
}
