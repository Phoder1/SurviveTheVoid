using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "FightState", menuName = "States/" + "FightState")]
public class FightMode : StateBase
{
    
    public override void OnUpdate()
    {
        Debug.Log("Fight!");
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
