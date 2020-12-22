using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InputManager : MonoBehaviour
{
    public static InputManager _instance;
    PlayerManager _playerManager;
    UImanager _uiManager;
    List<InputState> activeInputsLis = new List<InputState>();
    public bool a_Button,b_Button;
   
    

    //indicates building modes, destruction mode, etc'
    
    public enum InputState { A_pressed, B_Pressed, attack };
    
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
        _uiManager = UImanager._instance;
        
    }

    // Update is called once per frame
    void Update()
    {

 


    }
  
    
    public void OnClicked(string Button)
    {
        if (Input.touchCount > 0)
        {
        }

        
    }
    public void ButtonCheck(VirtualButton[] ButtonPressed)
    {
        if (ButtonPressed[0].IsPressed) { a_Button = true; } else a_Button=false;
        if (ButtonPressed[1].IsPressed) { b_Button = true; } else b_Button = false;


    }
    public Vector2 GetAxis(string requestedAxis1,string requestedAxis2)
    {

      //ControllersCheck that returns Vector2 
        Vector2 moveDirection= new Vector2(Input.GetAxis(requestedAxis1),Input.GetAxis(requestedAxis2))*Time.deltaTime;




        return moveDirection.normalized;
    } 
    
    
 
  

    //World to grid position can be found on the Grid manager
    /*private Vector2Int ScreenToGridPosition(Vector3 screenPosition) {

    }*/

  
   

}


