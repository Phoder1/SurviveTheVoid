using System.Collections;
   
using System.Collections.Generic;
using UnityEngine;

public class StateBase : ScriptableObject
{
    public PlayerManager _playerManager;
    public virtual void OnUpdate()
    {
        if (_playerManager == null)
        {
        _playerManager = PlayerManager._instance;
        }
    }
    public virtual void ButtonA()
    {
        _playerManager.check();
        Debug.Log("A"+this);
    }
    public virtual void ButtonB()
    {
        Debug.Log("B" + this);
    }
}