using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{

    GridManager gridManager;
    static StateBase currentState;
    PlayerStateMachine playerStateMachine;
    [SerializeField] VirtualJoystick vJ;
    public static InputState inputState;
    Vector2 touchPosition;
    TileHit newTileHit, currentTileHit;
    bool isBuildingAttached = false;

    bool isHoldingButton = false, stopHoldingButton = false, isButtonA;

    List<Vector2Int> TileList = new List<Vector2Int>();


    TileSlot tileSlotCache;
    public override void Init() {
        playerStateMachine = PlayerStateMachine.GetInstance;
        gridManager = GridManager._instance;
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




    // Update is called once per frame

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


    void BuildingStateOnTouch(Touch touch)
    {

        switch (touch.phase)
        {
            case TouchPhase.Began:
            case TouchPhase.Moved:


                touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                currentTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor);
                if (tileSlotCache == null || currentTileHit == null || currentTileHit.tile == null || gridManager.GetTileFromGrid(currentTileHit.gridPosition, TileMapLayer.Buildings) != null)
                    return;


                isBuildingAttached = true;

                if (newTileHit != null && newTileHit.gridPosition == currentTileHit.gridPosition)
                {

                    gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Buildings, false);
                    if (!TileList.Contains(currentTileHit.gridPosition))
                    {
                        TileList.Add(currentTileHit.gridPosition);
                    }
                }
                else
                {
                    if (TileList.Count > 0)
                    {
                        foreach (var tile in TileList)
                        {
                            if (tile == null)
                            {
                                continue;
                            }
                            gridManager.SetTile(null, tile, TileMapLayer.Buildings, false);
                        }
                        TileList.Clear();
                    }
                    newTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor);



                    if (newTileHit == null || gridManager.GetTileFromGrid(newTileHit.gridPosition, TileMapLayer.Buildings) != null)
                        gridManager.SetTile(null, currentTileHit.gridPosition, TileMapLayer.Buildings, false);

                    currentTileHit = newTileHit;
                }

                break;
        }
    }


    public void SetBuildingTile(TileAbstSO Item) {
        tileSlotCache = null;
        if (Item == null)
            return;


        tileSlotCache = new TileSlot(Item);
    }




    public Vector2 GetAxis()
    {

        //ControllersCheck that returns Vector2 
        Vector2 moveDirection = vJ.inpudDir;
        return moveDirection;
    }


    public void PressedConfirmBuildingButton(bool toConfirm)
    {
        if (toConfirm)
        {
            Touch newTouch = new Touch();
            if ((Vector2)Camera.main.ScreenToWorldPoint(newTouch.position) != touchPosition)
            {
                PlayerStateMachine.GetInstance.SwitchState(InputState.DefaultState);
            }
        }
    }


    void ButtonControls() {
        if (isHoldingButton)
        {
            ReleaseButton();
      
        }
    }




    public void ButtonPressedDown(bool _isButtonA)
    {
        this.isButtonA = _isButtonA;
        stopHoldingButton = false;
        if (_isButtonA)
            PressButtonA();
        
        else
            PressButtonB();
        
    } 
    public void ButtonPressedUp() {
       
        isHoldingButton = false;
        stopHoldingButton = true;
    }
    void ReleaseButton()
    {
        isHoldingButton = false;
        if (!stopHoldingButton)
        {
            if (isButtonA)
                Invoke("PressButtonA", .5f);
            
            else
                Invoke("PressButtonB", .5f);
        }
    }
    public void PressButtonB() {
        isHoldingButton = true;
        //  currentState.ButtonB();
        Debug.Log("Press Button B");
    }
     void PressButtonA() {
        isHoldingButton = true;
        Debug.Log("Press Button A");
        //currentState.ButtonA();
    }
    void Update()
    {
        ButtonControls();
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
}
