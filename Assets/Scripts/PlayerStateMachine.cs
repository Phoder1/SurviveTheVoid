using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class PlayerStateMachine 
{
    private static PlayerStateMachine _instance;

    public StateBase currentState;
    public StateBase[] stateArray;

    int counter=0; // ?****

    private PlayerStateMachine()
    {
        currentState = stateArray[counter];
    }
    public static PlayerStateMachine GetInstance {
        get {
            if (_instance == null)
            {
                _instance = new PlayerStateMachine();
            }
            return _instance;
        }
    }
    public StateBase SwichState(InputManager.InputState newState)
    {
        switch (newState)
        {
            case InputManager.InputState.DefaultMode:
                currentState = stateArray[0];
                break;

            case InputManager.InputState.BuildMode:
                currentState = stateArray[1];
                break;

            case InputManager.InputState.FightMode:
                currentState = stateArray[2];
                break;
        }
        return currentState;
    }
}
