using UnityEngine;
public class DefaultState : StateBase
{
    public override void ButtonB()
    {
        

        PlayerManager._instance.ImplementInteraction(false);
    }
}




