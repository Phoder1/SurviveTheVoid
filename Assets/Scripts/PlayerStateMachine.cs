public enum InputState { DefaultState, BuildState, FightState };
public class PlayerStateMachine 
{
    private static PlayerStateMachine _instance;
    StateBase[] stateBases = new StateBase[3];
    private PlayerStateMachine()
    {
        stateBases[0] = new DefaultState();
        stateBases[1] = new BuildingState();
        stateBases[2] = new FightState();
        SwitchState(InputState.DefaultState);

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
    public void SwitchState(InputState newState)
    {
        switch (newState)
        {
            case InputState.DefaultState:
                InputManager.SetInputState = stateBases[0];
                break;
            case InputState.BuildState:
                InputManager.SetInputState = stateBases[1];
                break;

            case InputState.FightState:
                InputManager.SetInputState = stateBases[2];
                break;
            default:
                    InputManager.SetInputState = stateBases[0];
                break;

        }
    }
}
