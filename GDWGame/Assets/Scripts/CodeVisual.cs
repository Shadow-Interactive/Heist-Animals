using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeVisual : MonoBehaviour {

    public RawImage[] codeSprites = new RawImage[4];
    [HideInInspector] int id = 0;

    public void SetSprite(int i, Texture newImage)
    {
        codeSprites[i].texture = newImage;
    }

    public void SetIndex(int i)
    {
        id = i;
    }

    public void SetActive(bool Active)
    {
        for (int i = 0; i < 4; i++)
            codeSprites[i].enabled = Active;
    }
}
