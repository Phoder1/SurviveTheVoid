using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InputManager : MonoBehaviour
{
    public static InputManager _instance;
    PlayerManager _playerManager;
    UIManager _uiManager;
   
    public bool a_Button,b_Button;
   
    


    public enum InputState { DefaultMode, BuildMode, FightMode };
    public InputState state;

    private void Awake() {
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
            state = InputState.DefaultMode;
            _playerManager.ChangeMode(state);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            state = InputState.BuildMode;
            _playerManager.ChangeMode(state);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            state = InputState.FightMode;
            _playerManager.ChangeMode(state);
        }


    }
  
    
    public void OnClicked(string Button,VirtualJoystick vJ)
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
    public void ButtonCheck(VirtualButton[] ButtonPressed)
    {
        if (ButtonPressed[0].IsPressed) { a_Button = true; } else a_Button=false;
        if (ButtonPressed[1].IsPressed) { b_Button = true; } else b_Button = false;


    }
    public Vector2 GetAxis()
    {
       
      //ControllersCheck that returns Vector2 
        Vector2 moveDirection= _uiManager.vJ.inpudDir;
        return moveDirection;
    } 
    
    
 
  

    //World to grid position can be found on the Grid manager
    /*private Vector2Int ScreenToGridPosition(Vector3 screenPosition) {

    }*/

  
   

}


