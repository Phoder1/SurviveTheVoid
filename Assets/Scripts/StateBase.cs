
using UnityEngine;

public abstract class StateBase
{
    public PlayerManager _playerManager;
    public virtual void OnUpdate() ////???????
    {
        if (_playerManager == null)
        {
        _playerManager = PlayerManager._instance;
        }
    }
    public void ButtonA()
    {
        _playerManager.Check();
        Debug.Log("A"+this);
    }
    public abstract void ButtonB();

}
public class FightState : StateBase
{
    public override void ButtonB()
    {
        
    }
}

public class DefaultState : StateBase
{
    public override void ButtonB()
    {
        throw new System.NotImplementedException();
    }
}

public class BuildingState : StateBase
{
    public override void ButtonB()
    {
        throw new System.NotImplementedException();
    }
}