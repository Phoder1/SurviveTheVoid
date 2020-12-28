using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BuildState", menuName = "States/" + "BuildState")]
public class BulidMode : StateBase
{
    
    public override void OnUpdate()
    {
        Debug.Log("Build!");
        base.OnUpdate();
        
    }
    public override void ButtonA()
    {
        
        base.ButtonA();
    }
    public override void ButtonB()
    {
        base.ButtonB();
    }

}
