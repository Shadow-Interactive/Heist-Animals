using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TheBulletPool : NetworkBehaviour {

    public GameObject Zapper;
    int numBullets = 15;
    List<GameObject> theBullets = new List<GameObject>();

    //https://docs.unity3d.com/Manual/UNetCustomSpawning.html for object pooling over network

    // Use this for initialization
    void Start() {
        for (int i = 0; i < numBullets; i++)
        {
            GameObject Bullet = Instantiate(Zapper, Vector3.zero, Quaternion.identity);
            theBullets.Add(Bullet);
        }
    }

    public GameObject availableBullet(Vector3 position)
    {
        //grab an available bullet
        foreach (var bullet in theBullets)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.GetComponent<ZapperScript>().SetPosition(position, Quaternion.identity);
                bullet.GetComponent<ZapperScript>().SetActive(true);
                return bullet;
            }
        }
        //otherwise return null
        return null;
    }

    //set object to false to return to pool
    public void returnToPool(GameObject bullet)
    {
        bullet.SetActive(false);
    }
}

