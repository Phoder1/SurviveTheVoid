using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase : ScriptableObject
{
    public virtual void OnUpdate()
    {
        Debug.Log("BaseLine");
    }
}
