using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scan;
using Assets.TilesData;

public class PlayerManager : MonoBehaviour 
{
    public static PlayerManager _instance;
    InputManager _inputManager;
    UIManager _uiManager;
    PlayerStateMachine _playerStateMachine;
    GridManager _GridManager;
    Scanner _scanner;

    internal StateBase myState;
    [SerializeField] internal Camera cameraComp;

    Vector2 cameraRealSize => new Vector2(cameraComp.orthographicSize * 2 * cameraComp.aspect, cameraComp.orthographicSize * 2);
    Vector2 movementVector;
    Vector2 currentPos;
    Vector2 nextPos;
    Vector2Int lastPosOnGrid;
    TileMapLayer buildingLayer;



    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);

        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
       
        buildingLayer = TileMapLayer.Buildings;
        _scanner = new Scanner();
        _inputManager = InputManager._instance;
        _GridManager = GridManager._instance;
        _uiManager = UIManager._instance;
        lastPosOnGrid = new Vector2Int(int.MaxValue, int.MaxValue);


        _playerStateMachine = GetComponent<PlayerStateMachine>();
        ChangeMode(InputManager.InputState.DefaultMode);
       

        UpdateView();

    }

    // Update is called once per frame
    void Update()
    {
      
        


        TileHitStruct closestTile;
        
        movementVector = _inputManager.GetAxis();
        movementVector = movementVector * 5*Time.deltaTime;
        currentPos = (Vector2)transform.position; //new Vector2(transform.position.x, transform.position.y);
        nextPos = currentPos + movementVector;

        if ((movementVector != Vector2.zero && _GridManager.IsTileWalkable(nextPos, movementVector)) || Input.GetKey(KeyCode.LeftShift))
        {
            
            transform.Translate(movementVector);
            UpdateView();
            Vector2Int currentPosOnGrid = _GridManager.WorldToGridPosition(transform.position, buildingLayer);

            if (lastPosOnGrid != currentPosOnGrid)
            {
                
                closestTile = _scanner.Scan(currentPosOnGrid, DirectionEnum.Down, 5, TileMapLayer.Floor, new GatheringScanChecker());
                lastPosOnGrid = currentPosOnGrid;
                if (closestTile.tile != null)
                {
                    Debug.Log("dasdasd" + closestTile.gridPosition);
                    closestTile.tile.GatherInteraction(closestTile.gridPosition, buildingLayer);
                }
              
            }

        }

        if (_inputManager.a_Button) { ButtonA(); }
        if (_inputManager.b_Button) { ButtonB(); }
        

    }

    public void ButtonA()
    {
        
        myState.ButtonA();
    }
    public void ButtonB()
    {
        
        myState.ButtonB();
    }


    private void UpdateView()
    {
        Vector3 camPosition = (Vector2)transform.position;
        camPosition -= (Vector3)cameraRealSize / 2;
        Rect worldView = new Rect(camPosition, cameraRealSize);
        _GridManager.UpdateView(worldView);
    }

    public void ChangeMode(InputManager.InputState newState)
    {
        myState = _playerStateMachine.SwichState(newState);   
        
        myState.OnUpdate();
    }
    
    public void check()
    {
        Debug.Log("check Buttons");
    }
    public class GatheringScanChecker : IChecker
    {
        public bool CheckTile(GenericTile tile)
        {
            return !tile.isActiveInteraction;
        }
    }

}





