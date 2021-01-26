using UnityEngine;

public enum InputState { DefaultState, BuildState, FightState, RemovalState };
public class PlayerStateMachine
{
    private static PlayerStateMachine _instance;
    StateBase[] stateBases = new StateBase[4];
    private PlayerStateMachine()
    {
        stateBases[0] = new DefaultState();
        stateBases[1] = new BuildingState();
        stateBases[2] = new FightState();
        stateBases[3] = new RemovalState();
        SwitchState(InputState.DefaultState);

    }
    public static PlayerStateMachine GetInstance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerStateMachine();
            }
            return _instance;
        }
    }
    public void SwitchState(InputState newState)
    {
        UnityEngine.Debug.Log(newState);


        CameraController._instance.GetSetIsZoomedIn = false;
        switch (newState)
        {
            case InputState.DefaultState:


                InputManager.SetInputState = stateBases[0];

                break;
            case InputState.BuildState:

                InputManager.SetInputState = stateBases[1];
                CameraController._instance.GetSetIsZoomedIn = true;

                break;
            case InputState.FightState:

                InputManager.SetInputState = stateBases[2];
                

                break;
            case InputState.RemovalState:

                InputManager.SetInputState = stateBases[3];
                CameraController._instance.GetSetIsZoomedIn = true;

                break;
            default:
                InputManager.SetInputState = stateBases[0];
                break;

        }
        UIManager._instance.UpdateUiState(InputManager.inputState);
    }
}
