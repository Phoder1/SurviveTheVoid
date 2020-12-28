using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    PlayerManager _playerManager;
    public StateBase currentState;
    public StateBase[] stateArray;

    int counter=0;

    private void Start()
    {
        _playerManager = PlayerManager._instance;
        currentState = stateArray[counter];
        

    }
  
    public StateBase SwichState(InputManager.InputState newState)
    {
     
        SwichMode();

        void SwichMode()
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



        }
        return currentState;
    }
    public void Update()
    {
            
    }
}
