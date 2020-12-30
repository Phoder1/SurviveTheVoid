using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scan;
using Assets.TilesData;

public class PlayerManager : MonoBehaviour 
{
    public static PlayerManager _instance;
    private InputManager _inputManager;
    private UIManager _uiManager;
    private PlayerStateMachine _playerStateMachine;
    private GridManager _GridManager;
    private Scanner _scanner;

    internal StateBase myState;
    [SerializeField] internal Camera cameraComp;

    private Vector2 GetCameraRealSize => new Vector2(cameraComp.orthographicSize * 2 * cameraComp.aspect, cameraComp.orthographicSize * 2);

    private Vector2 movementVector;
    private Vector2 currentPos;
    private Vector2 nextPos;
    private Vector2Int lastPosOnGrid;
    private TileMapLayer buildingLayer;
    private TileHitStruct closestTile;
    private DirectionEnum movementDir;



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
    private void Start()
    {
       
        buildingLayer = TileMapLayer.Floor;
        _scanner = new Scanner();
        _inputManager = InputManager._instance;
        _GridManager = GridManager._instance;
        _uiManager = UIManager._instance;
        
        


        _playerStateMachine = GetComponent<PlayerStateMachine>();
        ChangeMode(InputManager.InputState.DefaultMode);
       

        UpdateView();

    }

    // Update is called once per frame
    private void Update()
    {
        movementVector = _inputManager.GetAxis();
        movementVector *= (5 * Time.deltaTime);
        currentPos = transform.position; //new Vector2(transform.position.x, transform.position.y);
        nextPos = currentPos + movementVector;

        if ((movementVector != Vector2.zero && _GridManager.IsTileWalkable(nextPos, movementVector)) || Input.GetKey(KeyCode.LeftShift))
        {
            switch (_inputManager.GetAxis())
            {
                //case :
                    //break;
            }
            
            transform.Translate(movementVector);
            UpdateView();
            Vector2Int currentPosOnGrid = _GridManager.WorldToGridPosition(transform.position, buildingLayer);

            if (lastPosOnGrid != currentPosOnGrid && Vector2.Distance(currentPosOnGrid, closestTile.gridPosition) > Vector2.Distance(lastPosOnGrid, closestTile.gridPosition))
            {
                Debug.Log("checkTile");
                closestTile = _scanner.Scan(currentPosOnGrid, DirectionEnum.Down, 5, buildingLayer, new GatheringScanChecker());
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
        Debug.Log(_GridManager.GridToWorldPosition(closestTile.gridPosition, buildingLayer,true));
       // transform.Translate(transform.position-_GridManager.GridToWorldPosition(closestTile.gridPosition, buildingLayer));
        myState.ButtonA();
    }
    public void ButtonB()
    {
        
        myState.ButtonB();
    }


    private void UpdateView()
    {
        Vector3 camPosition = (Vector2)transform.position;
        camPosition -= (Vector3)GetCameraRealSize / 2;
        Rect worldView = new Rect(camPosition, GetCameraRealSize);
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
        public bool CheckTile(TileAbst tile)
        {
            return !tile.isActiveInteraction;
        }
    }

}





