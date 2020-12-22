using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    private float scrollSpeed;
    [SerializeField]
    private float moveSpeed;
    private Vector2 movement;
    private float scrollMovement;
    private Camera cameraComp;
    private GridManager gridManager;
    Vector2 cameraRealSize => new Vector2(cameraComp.orthographicSize * 2 * cameraComp.aspect, cameraComp.orthographicSize * 2);
    Rect worldView;
    bool viewChanged;

    // Start is called before the first frame update
    void Start() {
        Init();
        gridManager.UpdateView(UpdateWorldView());
    }
    private void Init() {
        cameraComp = GetComponent<Camera>();
        gridManager = GridManager._instance;
    }

    // Update is called once per frame
    void Update()
    {
        viewChanged = false;
        movement = new Vector2(
            Input.GetKey(KeyCode.D)?1:0 - (Input.GetKey(KeyCode.A)?1:0),
            Input.GetKey(KeyCode.W)?1:0 - (Input.GetKey(KeyCode.S)?1:0)
            );
        movement *= moveSpeed;
        if(movement != Vector2.zero) {
            transform.position += (Vector3)movement;
            viewChanged = true;
        }

        scrollMovement = scrollSpeed * -Input.GetAxis("Mouse ScrollWheel");
        if(scrollMovement != 0) {
            cameraComp.orthographicSize += scrollMovement;
            viewChanged = true;
        }

        if (Input.GetKey(KeyCode.Mouse0)) {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                GridManager._instance.SetTile(gridManager.WorldToGridPosition(mousePos), new Tiles.ToothPaste());
            
        }
        else if (Input.GetKey(KeyCode.Mouse1)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            GridManager._instance.SetTile(gridManager.WorldToGridPosition(mousePos), null);
        }

        if (viewChanged) {
            gridManager.UpdateView(UpdateWorldView());
        }
    }

    private Rect UpdateWorldView() {
        Vector3 camPosition = (Vector2)transform.position ;
        camPosition -= (Vector3)cameraRealSize/2;
        worldView = new Rect(camPosition, cameraRealSize);
        return worldView;
    }
}
