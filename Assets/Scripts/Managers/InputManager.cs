
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager _instance;
    PlayerManager _playerManager;
    UIManager _uiManager;
    StateBase currentState;
    PlayerStateMachine playerStateMachine;
    public bool a_Button, b_Button;
   [SerializeField] VirtualJoystick vJ;

    private void Awake() {
        playerStateMachine = PlayerStateMachine.GetInstance;
        currentState = playerStateMachine.SwichState(InputState.DefaultMode);

        if (_instance != null) {
            Destroy(gameObject);

        }
        else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        _playerManager = PlayerManager._instance;
        _uiManager = UIManager._instance;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
           UpdateState(InputState.BuildMode);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateState(InputState.DefaultMode);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {

           UpdateState(InputState.FightMode);
        }

        OnTouch("sdas", vJ);
    }

    // need to implement touch and use on phone 
    public void OnTouch(string Button, VirtualJoystick vJ)
    {
        Touch[] touch = new Touch[5];
        //if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                touch[i] = Input.GetTouch(i);
                if (touch[i].position == new Vector2(vJ.gameObject.transform.position.x, vJ.gameObject.transform.position.y) || new Vector2(Input.mousePosition.x, Input.mousePosition.y) == new Vector2(vJ.gameObject.transform.position.x, vJ.gameObject.transform.position.y))
                {


                }

            }
        }

    }
  
    public Vector2 GetAxis()
    {

        //ControllersCheck that returns Vector2 
        Vector2 moveDirection = vJ.inpudDir;
        return moveDirection;
    }





    //World to grid position can be found on the Grid manager
    /*private Vector2Int ScreenToGridPosition(Vector3 screenPosition) {

    }*/

    public void UpdateState(InputState _state) {
        currentState = playerStateMachine.SwichState(_state);
    }

    public void PressButtonA() {
        currentState.ButtonA();
    } 
    public void PressButtonB() {
        currentState.ButtonB();
    }
}


