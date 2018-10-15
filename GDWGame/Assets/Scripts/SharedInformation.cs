using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedInformation : MonoBehaviour {

    public enum TypesOfTraps
    {
        NO_TRAP = 0,
        SHOCK = 1,
        EMP = 2
    }

    [HideInInspector] public string strZap = "Zap";
    
}
