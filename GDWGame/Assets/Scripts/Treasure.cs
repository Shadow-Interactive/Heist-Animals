using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour {

    public Objective minigame;

    public void TreasureOnClick(Texture[] theImages)
    {
        minigame.ActivateObject(theImages);
    }
}
