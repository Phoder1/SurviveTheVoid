using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager _instance; 

    //indicates building modes, destruction mode, etc'
    private enum InputState { };
    private InputState state;

    private void Awake() {
        if (_instance == null) {
            Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
        else {
            _instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //World to grid position can be found on the Grid manager
    /*private Vector2Int ScreenToGridPosition(Vector3 screenPosition) {

    }*/

    //
    

}
