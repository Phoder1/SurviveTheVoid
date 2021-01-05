using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scan;
using System;

public class PlayerManager : MonoSingleton<PlayerManager> 
{
    private InputManager _inputManager;
    private UIManager _uiManager;
    private PlayerStateMachine _playerStateMachine;
    private GridManager _GridManager;
    private Scanner _scanner;

   
    [SerializeField] internal Camera cameraComp;

    private Vector2 GetCameraRealSize => new Vector2(cameraComp.orthographicSize * 2 * cameraComp.aspect, cameraComp.orthographicSize * 2);

    private Vector2 movementVector;
    private Vector2 currentPos;
    private Vector2 nextPos;
    private Vector2Int lastPosOnGrid;
    private Vector2Int currentPosOnGrid;
    private TileMapLayer buildingLayer;
    private TileHit closestTile;
    private DirectionEnum movementDir;
    // Start is called before the first frame update
    public override void Init()
    {
       
        buildingLayer = TileMapLayer.Floor;
        _scanner = new Scanner();
        _inputManager = InputManager._instance;
        _GridManager = GridManager._instance;
        _uiManager = UIManager._instance;
        

        UpdateView();

    }

    // Update is called once per frame
    private void Update()
    {
        movementVector = _inputManager.GetAxis();
        movementVector *= (5 * Time.deltaTime);
        currentPos = transform.position; //new Vector2(transform.position.x, transform.position.y);
        nextPos = currentPos + movementVector;
        FindDirection();
        if ((movementVector != Vector2.zero && _GridManager.IsTileWalkable(nextPos, movementVector)) || Input.GetKey(KeyCode.LeftShift))
        {
            //switch (_inputManager.GetAxis())
            //{
            //    //case :
            //        //break;
            //}
            
            transform.Translate(movementVector);
            UpdateView();
            currentPosOnGrid = _GridManager.WorldToGridPosition(transform.position, buildingLayer);

          

        }

       
        

    }
    public void Scan(IChecker checkType)
    {
        if (closestTile == null)
        {
            closestTile = _scanner.Scan(currentPosOnGrid, DirectionEnum.Up, 5, buildingLayer, checkType);
            lastPosOnGrid = currentPosOnGrid;
        }
        else
        {

            float posToClosestDis = Vector2.Distance(currentPos, _GridManager.GridToWorldPosition(closestTile.gridPosition, buildingLayer, true));
            float lastposToClosestDis = Vector2.Distance(_GridManager.GridToWorldPosition(lastPosOnGrid, buildingLayer, true), _GridManager.GridToWorldPosition(closestTile.gridPosition, buildingLayer, true));
            if (lastPosOnGrid != currentPosOnGrid && (posToClosestDis > lastposToClosestDis))
            {
                Debug.Log("checkTile");
                closestTile = _scanner.Scan(currentPosOnGrid, DirectionEnum.Up, 5, buildingLayer, checkType);
                lastPosOnGrid = currentPosOnGrid;

                if (closestTile != null)
                {
                    //Check if Scanned-Do Not Delete!!//
                    // closestTile.tile.GatherInteraction(closestTile.gridPosition, buildingLayer);
                }
            }
        }
    }

    private void FindDirection()
    {
        float angle = Vector2.SignedAngle(_inputManager.GetAxis(), Vector2.up);
        int direction = Mathf.RoundToInt(angle / 90);

        


        switch (direction)
        {
            case 0:
                movementDir = DirectionEnum.Up;
                break;
            case 1:
                movementDir = DirectionEnum.Right;
                break;
            case -1:
                movementDir = DirectionEnum.Left;
                break;
            case 2:
                movementDir = DirectionEnum.Down;
                break;
            case -2:
                movementDir = DirectionEnum.Down;
                break;


        }
    }
    public void ImplementGathering()
    {

        Scan(new GatheringScanChecker());
        if (closestTile != null)
        {
            Vector3 destination = _GridManager.GridToWorldPosition(closestTile.gridPosition, buildingLayer, true);
            destination.z = transform.position.z;
            WalkTowards(destination);


        }
        
        // transform.Translate(transform.position-_GridManager.GridToWorldPosition(closestTile.gridPosition, buildingLayer));
    }
    public void ImplementSpecialInteraction()
    {
        Scan(new SpecialInterractionScanChecker());
        if (closestTile != null)
        {
            Vector3 destination = _GridManager.GridToWorldPosition(closestTile.gridPosition, buildingLayer, true);
            destination.z = transform.position.z;
            WalkTowards(destination);


        }
     
    }


    private void UpdateView()
    {
        Vector3 camPosition = (Vector2)transform.position;
        camPosition -= (Vector3)GetCameraRealSize / 2;
        Rect worldView = new Rect(camPosition, GetCameraRealSize);
        _GridManager.UpdateView(worldView);
    }
    public class GatheringScanChecker : IChecker
    {
        public bool CheckTile(TileSlot tile)
        {
            return tile.GetInteractionType != InteractionType.Special;
        }
    }
    public class SpecialInterractionScanChecker : IChecker
    {
        public bool CheckTile(TileSlot tile)
        {
            return tile.GetInteractionType == InteractionType.Special;
        }
    }
    public void WalkTowards(Vector3 destination)
    {
        if (_GridManager.WorldToGridPosition(transform.position, buildingLayer) != closestTile.gridPosition)
        {

            transform.Translate((destination - transform.position) * Time.fixedDeltaTime);
            
            
        }
        else
        {
            
            closestTile.tile.GatherInteraction(closestTile.gridPosition, buildingLayer);
            Debug.Log("TileHarvested");
            closestTile = null;
        }
        
        
    }

}





