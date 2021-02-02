using UnityEngine;


public class InputManager : MonoSingleton<InputManager>
{
    [SerializeField] bool useMouse = false; 
    GridManager gridManager;
    static StateBase currentState;
    PlayerStateMachine playerStateMachine;
    [SerializeField] VirtualJoystick vJ;
    public static InputState inputState;
    public Vector2 VJAxis => vJ.JoystickVector;


    Touch?[] touch;
    bool[] touchNumberIsVJ;
    int? vjIndex;
    public int? GetVJIndex => vjIndex;
    public override void Init()
    {
        playerStateMachine = PlayerStateMachine.GetInstance;
        gridManager = GridManager._instance;
        touch = new Touch?[3];
        touchNumberIsVJ = new bool[touch.Length];
        for (int i = 0; i < touch.Length; i++)
        {
            touch[i] = null;
            touchNumberIsVJ[i] = false;
        }
      
        DeathReset();

    }
  public  bool IsAlreadyAcitve() {

        for (int i = 0; i < touchNumberIsVJ.Length; i++)
        {
            if (touchNumberIsVJ[i])
                return true;
        }
        return false;
    
    }
    void ResetBoolArray()
    {
        for (int i = 0; i < touchNumberIsVJ.Length; i++)
            if (touchNumberIsVJ[i] == true)  touchNumberIsVJ[i] = false;

        vjIndex = null;
    }
    private void AssignTouch()
    {
        ResetBoolArray();
        if (0 == Input.touchCount || touch==null)
            return ;
        for (int i = 0; i < touch.Length; i++)
        {

            if (i < Input.touchCount)
                touch[i] = Input.GetTouch(i);

            
            

            if (vJ.GetVJActivity && !IsAlreadyAcitve()) { 
                touchNumberIsVJ[i] = true;
                vjIndex = i;
            }

        }


    }
    bool isMovingWithJoystick;
    

    public static StateBase SetInputState
    {
        set
        {
            if (currentState != value)
            {
                if (currentState!= null)
                {
                currentState.OnSwitchState();

                }
                 currentState = value;
            }

            switch (currentState) {
                case BuildingState buildingState:
                    inputState = InputState.BuildState;
                    UIManager._instance.SetScreenOutlineColor(Color.green);
                    break;
                case FightState fightState:
                    inputState = InputState.FightState;
                    UIManager._instance.SetScreenOutlineColor(Color.clear);
                    break;
                case RemovalState removalState:
                    inputState = InputState.RemovalState;
                    UIManager._instance.SetScreenOutlineColor(Color.red);
                    break;

                default:
                    inputState = InputState.DefaultState;
                    UIManager._instance.SetScreenOutlineColor(Color.clear);
                    break;
            }
        }
    }

    public static StateBase GetCurrentState => currentState;
    public void OnTouch()
    {
      
        if (Input.touchCount > 0)
        {

            AssignTouch();



            for (int i = 0; i < Input.touchCount; i++)
            {
                if (i >= touch.Length)
                    return;


                if (vjIndex != null && i == vjIndex) {
               
                    if (Input.touchCount == 1 && currentState is BuildingState)
                        (currentState as BuildingState).BuildWithVJ(PlayerManager.GetGridMovement);
                       
                    

                
                    continue;
                }



                if ((touch[i] == null))
                    return;
                

                switch (inputState)
                {
                    case InputState.DefaultState:
                        // default state
                        break;
                    case InputState.BuildState:
                        currentState.StateOnTouch(touch[i].GetValueOrDefault());
                       
                        break;
                    case InputState.FightState:
                        // fightState
                        break;
                    case InputState.RemovalState:
                        currentState.StateOnTouch(touch[i].GetValueOrDefault());
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            if (!useMouse)
                return; 



            switch (inputState)
            {
                case InputState.DefaultState:
              
                    break;
                case InputState.BuildState:
                    currentState.MousePos();

                    break;
                case InputState.FightState:
                    // fightState
                    break;
                case InputState.RemovalState:
                    currentState.MousePos();
                    break;
                default:
                    break;
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


    public void DeathReset() => playerStateMachine.SwitchState(InputState.DefaultState);

}
