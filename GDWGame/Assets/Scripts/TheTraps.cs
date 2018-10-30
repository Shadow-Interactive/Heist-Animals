using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrapBase : MonoBehaviour {

    [HideInInspector] public Mesh theMesh;
    [HideInInspector] public Material theMaterial;

    public virtual void Execute(PlayerObjectiveManager thePlayer){}

    public void SetProperties(Mesh newMesh, Material newMaterial)
    {
        theMesh = newMesh;
        theMaterial = newMaterial;
    }
}

public class WiredCase : TrapBase
{
    public override void Execute(PlayerObjectiveManager thePlayer)
    {
        thePlayer.WiredCaseTrap();
    }
}

public class ReinforcedCase : TrapBase
{
    public override void Execute(PlayerObjectiveManager thePlayer)
    {
        thePlayer.ReinforcedCaseTrap();
    }
}

public class SmokeBomb : TrapBase
{
    public override void Execute(PlayerObjectiveManager thePlayer)
    {
        thePlayer.SmokeBombTrap();
    }
}
