using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThisIsBad : MonoBehaviour {

    [HideInInspector] public Quaternion theRotation;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //https://answers.unity.com/questions/1421647/my-script-to-make-an-object-bob-up-and-down-as-an.html
        transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.time) * 0.15f + 3.1f), transform.position.z);
    }
}
