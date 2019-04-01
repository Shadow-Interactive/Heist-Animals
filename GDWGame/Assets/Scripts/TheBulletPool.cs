using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TheBulletPool : NetworkBehaviour {

    //we can keep weapons in this class
    //bullet related variables
    public GameObject Zapper;
    int numBullets = 15;
    List<GameObject> theBullets = new List<GameObject>();

    //smoke bomb related variables
    public GameObject SmokeBomb;
    int numSmokeBombs = 4;
    List<GameObject> sBombs = new List<GameObject>();

    //https://docs.unity3d.com/Manual/UNetCustomSpawning.html for object pooling over network

    // Use this for initialization
    void Start() {
        //initialize bullet pool
        for (int i = 0; i < numBullets; i++)
        {
            GameObject Bullet = Instantiate(Zapper, Vector3.zero, Quaternion.identity);
            theBullets.Add(Bullet);
        }

        //initialize smokebomb pool
        for (int i = 0; i < numSmokeBombs; i++)
        {
            GameObject SB = Instantiate(SmokeBomb, Vector3.zero, Quaternion.identity);
            sBombs.Add(SB);
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

    public GameObject availableSmokeBomb(Vector3 position)
    {
        //grab an available bullet
        foreach (var smokebomb in sBombs)
        {
            if (!smokebomb.activeInHierarchy)
            {
                smokebomb.GetComponent<SmokeBomb>().SetPosition(position, Quaternion.identity);
                smokebomb.GetComponent<SmokeBomb>().SetActive(true);
                return smokebomb;
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

    public void returnSmokeToPool(GameObject smoke)
    {
        smoke.SetActive(false);
    }
    
}

