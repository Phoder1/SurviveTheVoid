
using UnityEngine;

public class InputManager : MonoBehaviour
{
   
    public static InputManager _instance;
    GridManager gridManager;
    static StateBase currentState;
    PlayerStateMachine playerStateMachine;
    [SerializeField] VirtualJoystick vJ;
    public static InputState inputState;
    Vector2 touchPos, startTouchPos;
    TileHit newTileHit, previousTileHit;
    bool isBuildingAttached = false;
    
    private void Awake() {
        playerStateMachine = PlayerStateMachine.GetInstance;
        gridManager = GridManager.GetInstance;

        if (_instance != null) {
            Destroy(gameObject);

        }
        else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public static StateBase SetInputState
        {
        set {
            currentState = value;

            if (currentState is BuildingState)
                inputState = InputState.BuildState;

            else if (currentState is FightState)
                inputState = InputState.FightState;

            else
                inputState = InputState.DefaultState;
            
        }
    }



    // Start is called before the first frame update
    void Start()
    {
     
        gridManager = GridManager.GetInstance;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerStateMachine.SwitchState(InputState.DefaultState);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerStateMachine.SwitchState(InputState.BuildState);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            playerStateMachine.SwitchState(InputState.FightState);
        }

        OnTouch();
    }

    // need to implement touch and use on phone 
    public void OnTouch()
    {
        if (Input.touchCount > 0)
        {
            
            Touch[] touch = new Touch[3];


            Debug.Log(Input.touchCount);
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
                        BuildingStateOnTouch(touch[i]);
                        break;
                    case InputState.FightState:
                        // fightState
                        break;
                    default:
                        break;
                }




            }
        }

    }


    void BuildingStateOnTouch(Touch touch) {

        switch (touch.phase)
        {
            case TouchPhase.Began:

                startTouchPos = Camera.main.ScreenToWorldPoint(touch.position);

                    previousTileHit = gridManager.GetHitFromWorldPosition(startTouchPos, TileMapLayer.Floor);
                    TileSlot buildTileToPlace = previousTileHit.tile;

                    if (buildTileToPlace == null)
                        return;

                    isBuildingAttached = true;
                break;




            case TouchPhase.Stationary:
            case TouchPhase.Moved:
                { 
                    if (previousTileHit.tile == null)
                        return;

                    touchPos = Camera.main.ScreenToWorldPoint(touch.position);

                    newTileHit = gridManager.GetHitFromWorldPosition(touchPos, TileMapLayer.Floor);

                    if (newTileHit == null)
                        return;



                    if (newTileHit.gridPosition == previousTileHit.gridPosition)
                    {
                        //  gridManager.SetTile(buildTileToPlace,previousTileHit.gridPosition, TileMapLayer.Buildings, true);
                        
                        //  var buildTileToPlace.position = previousTileHit.gridPosition;   // 
                        //     tileHit.gridPosition;
                    }
                    else
                    {
                        previousTileHit = newTileHit;
                   
                    }


                }
                break;
            

               
            case TouchPhase.Ended:
            case TouchPhase.Canceled:


                if (isBuildingAttached)
                {
                    // wait till the player confirm the building position
                    //  gridManager.SetTile(buildTileToPlace,previousTileHit.gridPosition, TileMapLayer.Buildings, true);
            
                    isBuildingAttached = false;
                }


                break;

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



    public void PressButtonA() {
        currentState.ButtonA();
    } 
    public void PressButtonB() {
        currentState.ButtonB();
    }
}
