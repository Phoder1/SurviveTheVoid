using UnityEditor.Compilation;
using UnityEngine;

public abstract class StateBase
{
    public virtual void ButtonA()
    {
        Debug.Log("Implement ButtonA");
        PlayerManager._instance.ImplementGathering();
    }
    public abstract void ButtonB();
    public virtual void StateOnTouch(Touch touch) {
    
    
    
    }
}

