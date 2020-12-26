using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public StateBase currentState;
    public StateBase[] stateArray;

    int counter=0;

    private void Start()
    {
        currentState = stateArray[counter];
        
    }
    public void Update()
    {


        if (Input.GetKeyDown(KeyCode.Space))
        {
            counter++;
            if (counter >= stateArray.Length)
            {
                counter = 0;
            }
            currentState = stateArray[counter];




        }
        currentState.OnUpdate();
    }
}
