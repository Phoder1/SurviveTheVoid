using UnityEngine;
public class DefaultState : StateBase
{
    public override void ButtonB()
    {
        Debug.Log("DefaultState");
        

        PlayerManager._instance.ImplementGathering();
    }
}




