using UnityEngine;

public abstract class StateBase
{
    public void ButtonA()
    {
        Debug.Log("Implement ButtonA");
        PlayerManager._instance.ImplementSpecialInteraction();
    }
    public abstract void ButtonB();

}
