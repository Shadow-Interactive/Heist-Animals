using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour {

    string zapStr = "Zap";

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(zapStr))
        {
            //print("ughitworks??");
           Vector3 contactPt = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
           // other.GetComponent<ZapperScript>().Ricochet(contactPt);
            // other.GetComponent<ZapperScript>().SetActive(false); //THIS WILL GET CLEANED UP I SWEARRR
            other.transform.rotation = new Quaternion(other.transform.rotation.x, other.transform.rotation.y-180,other.transform.rotation.z, 0);
        }


        //   Vector3.Reflect(other.GetComponent<Rigidbody>().velocity, other.cont);
    }
}
