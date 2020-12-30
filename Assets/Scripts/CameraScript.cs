using UnityEngine;
using Assets.TilesData;

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
    private Vector2 cameraRealSize => new Vector2(cameraComp.orthographicSize * 2 * cameraComp.aspect, cameraComp.orthographicSize * 2);
    private Rect worldView;
    private bool viewChanged;
    private Camera camera1;
    private TilesPackSO tilesPack;

    // Start is called before the first frame update
    private void Start() {
        camera1 = Camera.main;
        Init();
        UpdateView();
    }
    private void Init() {
        cameraComp = GetComponent<Camera>();
        gridManager = GridManager._instance;
        tilesPack = gridManager.tilesPack;
    }

    // Update is called once per frame
    private void Update() {
        viewChanged = false;
        movement = new Vector2(
            Input.GetKey(KeyCode.D) ? 1 : 0 - (Input.GetKey(KeyCode.A) ? 1 : 0),
            Input.GetKey(KeyCode.W) ? 1 : 0 - (Input.GetKey(KeyCode.S) ? 1 : 0)
            );
        movement *= moveSpeed * Time.deltaTime;
        if (movement != Vector2.zero) {
            transform.position += (Vector3)movement;
            viewChanged = true;
        }

        scrollMovement = Time.deltaTime *  scrollSpeed * -Input.GetAxis("Mouse ScrollWheel");
        if (scrollMovement != 0) {
            cameraComp.orthographicSize += scrollMovement;
            viewChanged = true;
        }

        if (Input.GetKey(KeyCode.Mouse0)) {
            TileMapLayer layer = (Input.GetKey(KeyCode.LeftShift)) ? TileMapLayer.Buildings : TileMapLayer.Floor;
            Vector2Int gridPosition = MouseGridPosition(TileMapLayer.Floor);
<<<<<<< HEAD
            gridManager.SetTile(tilesPack.GetObsidianTile, gridPosition,  layer);
=======
            gridManager.SetTile(tilesPack.getObsidianTile, gridPosition,  layer);
>>>>>>> master

        }
        else if (Input.GetKey(KeyCode.Mouse1)) {
            TileMapLayer layer = (Input.GetKey(KeyCode.LeftShift)) ? TileMapLayer.Buildings : TileMapLayer.Floor;
            Vector2Int gridPosition = MouseGridPosition(layer);
            gridManager.SetTile(null, gridPosition,  layer);

        }
        else if (Input.GetKeyDown(KeyCode.LeftControl)) {
            TileMapLayer layer = (Input.GetKey(KeyCode.LeftShift)) ? TileMapLayer.Buildings : TileMapLayer.Floor;
            Debug.Log(gridManager.GetTileFromGrid(MouseGridPosition(layer), layer));
        }
        else if (Input.GetKeyDown(KeyCode.Mouse2)) {
            Vector3 mousePos = camera1.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            TileMapLayer layer = (Input.GetKey(KeyCode.LeftShift)) ? TileMapLayer.Buildings : TileMapLayer.Floor;
            TileHitStruct hit = gridManager.GetHitFromClickPosition(mousePos, layer);

            Debug.Log(hit.tile);
            if (hit.tile != null
                && hit.tile.interactionType == ToolInteractionEnum.Any) {
                Debug.Log("Color change");
                hit.tile.GatherInteraction((Vector2Int)hit.gridPosition, layer);
            }
        }

        if (viewChanged) {
            UpdateView();
        }
    }
    private Vector2Int MouseGridPosition(TileMapLayer buildingLayer) {
        Vector3 mousePos = camera1.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return gridManager.WorldToGridPosition(mousePos, buildingLayer);
    }
    private void UpdateView() {
        Vector3 camPosition = (Vector2)transform.position;
        camPosition -= (Vector3)cameraRealSize / 2;
        worldView = new Rect(camPosition, cameraRealSize);
        gridManager.UpdateView(worldView);
    }
}
