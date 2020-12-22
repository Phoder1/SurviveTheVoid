using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager _instance;
    InputManager inputManager;
    Vector2 movementVector;
   
    
    



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
        inputManager = InputManager._instance;
        
       



    }

    // Update is called once per frame
    void Update()
    {

        //bools//
        movementVector= inputManager.GetAxis("Horizontal","Vertical");
        if (inputManager.a_Button) { ButtonA(); }
        if (inputManager.b_Button) { ButtonB(); }
        


        //states//
        switch (inputManager.state)
        {
            case InputManager.InputState.A_pressed:
                ButtonA();
                break;
            case InputManager.InputState.B_Pressed:    
                ButtonB();
                break;
            case InputManager.InputState.attack:
                
                break;
        }

        transform.Translate(movementVector*Time.deltaTime);
    }

    public void ButtonA()
    {
        Debug.Log("ButtonA pressed");
        
    }
    public void ButtonB()
    {
        Debug.Log("ButtonB pressed");
    }
}





