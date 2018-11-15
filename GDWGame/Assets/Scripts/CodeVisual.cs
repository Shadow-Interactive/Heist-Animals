using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeVisual : MonoBehaviour {

    public RawImage[] codeSprites;
    [HideInInspector] int index = 0;

    public void SetSprite(int i, Texture newImage)
    {
        codeSprites[i].texture = newImage;
    }

    public void SetIndex(int i)
    {
        index = i;
    }
}
