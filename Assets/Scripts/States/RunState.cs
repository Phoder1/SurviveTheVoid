using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class RunState : StateBase
{
    public int yotamosmamos;
    public override void OnUpdate()
    {
        Debug.Log("RunState");
        base.OnUpdate();

    }
}
