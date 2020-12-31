public enum InputState { DefaultMode, BuildMode, FightMode };
public class PlayerStateMachine 
{
    private static PlayerStateMachine _instance;

     StateBase currentState;

    private PlayerStateMachine()
    {
        currentState = SwichState(InputState.DefaultMode);
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
    public StateBase SwichState(InputState newState)
    {
        switch (newState)
        {
            case InputState.DefaultMode:
                currentState = new DefaultState();
                break;
            case InputState.BuildMode:
                currentState = new BuildingState();
                break;

            case InputState.FightMode:
                currentState = new FightState();
                break;
        }
        return currentState;
    }
}
