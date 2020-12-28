using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DefaultState", menuName = "States/" + "DefaultState")]
public class DefaulMode : StateBase
{
    
    public override void OnUpdate()
    {
        Debug.Log("Default");
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




