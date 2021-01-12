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
    Vector2 touchPosition;
    TileHit newTileHit, currentTileHit;
    bool isBuildingAttached = false;
    bool cameFromBuildingState;



    List<Vector2Int> TileList = new List<Vector2Int>();


    TileSlot tileSlotCache;
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




    // Update is called once per frame

    // need to implement touch and use on phone
    public void OnTouch()
    {
        Debug.Log(currentState);
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
                    case InputState.RemovalState:
                        RemovalStateOnTouch(touch[i]);
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
                                continue;

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

    void RemovalStateOnTouch(Touch touch)
    {

        switch (touch.phase)
        {
            case TouchPhase.Began:
            case TouchPhase.Moved:


                touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                currentTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Buildings);
                Debug.Log(currentTileHit);
                if (currentTileHit == null || currentTileHit.tile == null || gridManager.GetTileFromGrid(currentTileHit.gridPosition, TileMapLayer.Buildings) == null)
                    return;
                Debug.Log("Found!");
                tileSlotCache = currentTileHit.tile;
                break;
        }
    }

    public void SetBuildingTile(TileAbstSO Item)
    {
        tileSlotCache = null;
        if (Item == null)
            return;

        newTileHit = null;
        currentTileHit = null;
        tileSlotCache = new TileSlot(Item);
    }




    public Vector2 GetAxis()
    {

        //ControllersCheck that returns Vector2
        Vector2 moveDirection = vJ.inpudDir;
        return moveDirection;
    }

    public void ConfirmRemoval()
    {
        if (tileSlotCache == null)
            return;


        if (currentTileHit!= null && Inventory.GetInstance.AddToInventory(0, new ItemSlot(currentTileHit.tile.GetTileAbst, 1)))
        {

            gridManager.SetTile(null, currentTileHit.gridPosition, TileMapLayer.Buildings, true);
            CancelButtonChangeState(true);

        }
    }
    public void PressedConfirmBuildingButton()
    {
        if (!isBuildingAttached || tileSlotCache == null || currentTileHit==null)
            return;


        Touch newTouch = new Touch();
        if ((Vector2)Camera.main.ScreenToWorldPoint(newTouch.position) != touchPosition)
        {
            gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Buildings, true);
            newTileHit = null;
            currentTileHit = null;
            var itemSlotCache = new ItemSlot(tileSlotCache.GetTileAbst, 1);
            Inventory.GetInstance.RemoveItemFromInventory(0, itemSlotCache);
            if (Inventory.GetInstance.GetAmountOfItem(0, itemSlotCache) >= 1)
            {
                SetBuildingTile(itemSlotCache.item as TileAbstSO);
            }
            else
            {
                PlayerStateMachine.GetInstance.SwitchState(InputState.DefaultState);
            tileSlotCache = null;

            }

            TileList.Clear();
            Debug.Log("Placed");
        }

    }

    public void CancelButtonChangeState(bool _cameFromBuildingState)
    {

        cameFromBuildingState = _cameFromBuildingState;

        if (cameFromBuildingState)
        {
            playerStateMachine.SwitchState(InputState.BuildState);
        }
        else
           currentState.ButtonB();
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerStateMachine.SwitchState(InputState.DefaultState);
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
