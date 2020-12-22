using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager _instance;
    InputManager inputManager;
    Vector2 movementVector;
    GridManager _GridManager;
    Vector2 currentPos;
    Vector2 nextPos;
    [SerializeField] private Camera cameraComp;

    Vector2 cameraRealSize => new Vector2(cameraComp.orthographicSize * 2 * cameraComp.aspect, cameraComp.orthographicSize * 2);










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
        _GridManager = GridManager._instance;

        UpdateView();






    }

    // Update is called once per frame
    void Update()
    {


        //bools//

        movementVector = inputManager.GetAxis("Horizontal", "Vertical");
        movementVector = movementVector * 5 * Time.deltaTime;
        currentPos = (Vector2)transform.position; //new Vector2(transform.position.x, transform.position.y);
        nextPos = currentPos + movementVector;

        if ((movementVector != Vector2.zero && _GridManager.IsTileWalkable(nextPos, movementVector)) || Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(movementVector);
            UpdateView();

        }
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


    }

    public void ButtonA()
    {
        Debug.Log("ButtonA pressed");

    }
    public void ButtonB()
    {
        Debug.Log("ButtonB pressed");
    }
    private void UpdateView()
    {
        Vector3 camPosition = (Vector2)transform.position;
        camPosition -= (Vector3)cameraRealSize / 2;
        Rect worldView = new Rect(camPosition, cameraRealSize);
        _GridManager.UpdateView(worldView);
    }
}





