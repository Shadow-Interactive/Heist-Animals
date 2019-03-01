using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThisIsBad : MonoBehaviour {

    [HideInInspector] public Quaternion theRotation;

    float yValue;
	// Use this for initialization
	void Start () {

        yValue = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        //https://answers.unity.com/questions/1421647/my-script-to-make-an-object-bob-up-and-down-as-an.html

        transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.time) * 0.15f + yValue), transform.position.z);
    }
}
