using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalState : StateBase
{
    public override void ButtonB()
    {
        InputManager._instance.ConfirmRemoval();
    }
}
