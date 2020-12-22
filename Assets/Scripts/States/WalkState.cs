using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class WalkState : StateBase
{
    public int yotamosmamos;
    public override void OnUpdate()
    {
        Debug.Log("Walkstate");
        base.OnUpdate();
        
    }

}
