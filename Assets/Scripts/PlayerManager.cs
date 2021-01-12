using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scan;
using System;

public class PlayerManager : MonoSingleton<PlayerManager> 
{
    [SerializeField] PlayerStats playerStats;

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


    [SerializeField] int lookRange=5;



 

    public override void Init()
    {
       
        buildingLayer = TileMapLayer.Buildings;
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
 
         //new Vector2(transform.position.x, transform.position.y);
        


        if (movementVector != Vector2.zero)
        {
            FindDirection();
            WalkTowards(movementVector, false);
        }

       
        

    }
    
    public void Scan(IChecker checkType)
    {
        currentPosOnGrid = _GridManager.WorldToGridPosition(new Vector3(transform.position.x,transform.position.y,0), TileMapLayer.Buildings);


        Debug.Log("checkTile");
        closestTile = _scanner.Scan(currentPosOnGrid, movementDir, lookRange, buildingLayer, checkType);
        lastPosOnGrid = currentPosOnGrid;
        Debug.Log(currentPosOnGrid+" "+ " "+movementDir + " "+lookRange + " "+buildingLayer + " "+ checkType + " " + closestTile);
       



       //to check if scan works//
       // closestTile.tile.GatherInteraction(closestTile.gridPosition, buildingLayer);

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


            }
        
    }
    public void ImplementGathering()
    {

        Scan(new GatheringScanChecker());
        if (closestTile != null)
        {
            Vector3 destination = _GridManager.GridToWorldPosition(closestTile.gridPosition, buildingLayer, true);
            destination.z = transform.position.z;
            WalkTowards(destination,true);


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
            WalkTowards(destination,true);



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
    public void WalkTowards(Vector3 destination,bool ScanMod)
    {

        if (ScanMod)
        {


            if (_GridManager.WorldToGridPosition(transform.position, buildingLayer) != closestTile.gridPosition)
            {

                transform.Translate((destination - transform.position) * (Time.fixedDeltaTime/playerStats.GetSetSpeed)); 



            }

            if (Vector2.Distance(transform.position, _GridManager.GridToWorldPosition(closestTile.gridPosition, buildingLayer, true)) < 0.3f)
            {

                closestTile.tile.GatherInteraction(closestTile.gridPosition, buildingLayer);
                Debug.Log(Vector2.Distance(transform.position, _GridManager.GridToWorldPosition(closestTile.gridPosition, buildingLayer, true)));
                Debug.Log("TileHarvested");
                closestTile = null;
            }
        }
        else 
        {
            destination *= (playerStats.GetSetSpeed * Time.deltaTime);
            currentPos = transform.position;
            nextPos = currentPos + new Vector2(destination.x,destination.y);
            if (_GridManager.IsTileWalkable(nextPos, destination) || Input.GetKey(KeyCode.LeftShift))
            {
            transform.Translate(destination);

            }
        }

        UpdateView();

    }

}





