using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class IdleState : StateBase
{
    public int yotamosmamos;
    public override void OnUpdate()
    {
        Debug.Log("Idle");
        base.OnUpdate();

    }
}




