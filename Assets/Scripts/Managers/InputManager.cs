using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{

    GridManager gridManager;
    static StateBase currentState;
    PlayerStateMachine playerStateMachine;
    [SerializeField] VirtualJoystick vJ;
    public static InputState inputState;
    public Vector2 VJAxis => vJ.inpudDir;

 
    public override void Init()
    {
        playerStateMachine = PlayerStateMachine.GetInstance;
        gridManager = GridManager._instance;
    }
    public static StateBase SetInputState
    {
        set
        {
            currentState = value;

            if (currentState is BuildingState)
                inputState = InputState.BuildState;

            else if (currentState is FightState)
                inputState = InputState.FightState;

            else if (currentState is RemovalState)
                inputState = InputState.RemovalState;

            else
                inputState = InputState.DefaultState;

        }
    }

    public static StateBase GetCurrentState => currentState;
    public void OnTouch()
    {
      
        if (Input.touchCount > 0)
        {

            Touch[] touch = new Touch[3];



            for (int i = 0; i < Input.touchCount; i++)
            {
                if (i >= touch.Length)
                    return;

                touch[i] = Input.GetTouch(i);

                if (touch[i].position == new Vector2(vJ.gameObject.transform.position.x, vJ.gameObject.transform.position.y) || new Vector2(Input.mousePosition.x, Input.mousePosition.y) == new Vector2(vJ.gameObject.transform.position.x, vJ.gameObject.transform.position.y))
                    continue;

                switch (inputState)
                {
                    case InputState.DefaultState:
                        // default state
                        break;
                    case InputState.BuildState:
                        currentState.StateOnTouch(touch[i]);
                       
                        break;
                    case InputState.FightState:
                        // fightState
                        break;
                    case InputState.RemovalState:
                        currentState.StateOnTouch(touch[i]);
                        break;
                    default:
                        break;
                }
            }
        }

    }
    


    public void SinglePressedButton(bool isButtonA)
    {

        if (isButtonA)
            currentState.ButtonA();
        else
            currentState.ButtonB();


    }


    public void HoldingButton(bool isButtonA)
    {

        switch (currentState)
        {
            case RemovalState s:
            case BuildingState z:
                return;
        }
        if (isButtonA)
            currentState.ButtonA();
        else
            currentState.ButtonB();
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerStateMachine.SwitchState(InputState.DefaultState);
            Debug.Log(currentState);
        }else
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerStateMachine.SwitchState(InputState.BuildState);
            Debug.Log(currentState);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            playerStateMachine.SwitchState(InputState.FightState);
            Debug.Log(currentState);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            playerStateMachine.SwitchState(InputState.RemovalState);
            Debug.Log(currentState);
        }

        OnTouch();
    }
}
