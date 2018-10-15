using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZapperScript : MonoBehaviour {

    [HideInInspector] public string zapperTag;

	// Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
        gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 4;
    }

}
