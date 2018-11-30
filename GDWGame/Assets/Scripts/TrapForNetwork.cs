using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TrapForNetwork : NetworkBehaviour {

    //these variables will track what trap the runners are interacting with
    //need two so that runner 1 and runner 2 are being tracked seperately
    //with only one variable, if both runners were interacting with different traps at the same time, the variable would be overwritten
    //basically, no bueno
    [SyncVar]
    public int trapToGoAway;
    [SyncVar]
    public int otherTrapToGoAway;
}
