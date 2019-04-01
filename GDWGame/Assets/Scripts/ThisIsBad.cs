using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThisIsBad : MonoBehaviour {

    [HideInInspector] public Quaternion theRotation;
    public bool activateRotation = false;

    float yValue;
    public float multAmount = 0.15f;

	// Use this for initialization
	void Start () {

        yValue = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        //link for the bobbing part
        //https://answers.unity.com/questions/1421647/my-script-to-make-an-object-bob-up-and-down-as-an.html

        transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.time) * multAmount + yValue), transform.position.z);

        //rotates if you set it to rotate
        if (activateRotation)
        {
            transform.Rotate(0, Time.deltaTime*4.5f, 0);
        }
    }
}
