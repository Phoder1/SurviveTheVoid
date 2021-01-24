using UnityEngine;

public class GodmodeScript : MonoSingleton<GodmodeScript>
{
    [SerializeField]
    private float scrollSpeed;
    [SerializeField]
    private float moveSpeed;
    [SerializeField] 
    private BlockTileSO clickTile;
    private Vector2 movement;
    private float scrollMovement;
    GridManager gridManager;
    private bool viewChanged;
    CameraController cameraController;




    // Start is called before the first frame update
    public override void Init() {
        gridManager = GridManager._instance;
        cameraController = CameraController._instance;
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

        scrollMovement = Time.deltaTime * scrollSpeed * -Input.GetAxis("Mouse ScrollWheel");
        if (scrollMovement != 0) {
            viewChanged = true;

        }

        if (Input.GetKey(KeyCode.Mouse0)) {
            TileMapLayer layer = (Input.GetKey(KeyCode.LeftShift)) ? TileMapLayer.Buildings : TileMapLayer.Floor;
            Vector2Int gridPosition = MouseGridPosition(TileMapLayer.Floor);
            gridManager.SetTile(new TileSlot(clickTile), gridPosition, layer);


        }
        else if (Input.GetKey(KeyCode.Mouse1)) {
            TileMapLayer layer = (Input.GetKey(KeyCode.LeftShift)) ? TileMapLayer.Buildings : TileMapLayer.Floor;
            Vector2Int gridPosition = MouseGridPosition(layer);
            gridManager.SetTile(null, gridPosition, layer);

        }
        else if (Input.GetKeyDown(KeyCode.LeftControl)) {
            TileMapLayer layer = (Input.GetKey(KeyCode.LeftShift)) ? TileMapLayer.Buildings : TileMapLayer.Floor;
            Debug.Log(gridManager.GetTileFromGrid(MouseGridPosition(layer), layer));
        }
        else if (Input.GetKeyDown(KeyCode.Mouse2)) {
            Vector3 mousePos = GetcameraComp().ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            TileMapLayer layer = (Input.GetKey(KeyCode.LeftShift)) ? TileMapLayer.Buildings : TileMapLayer.Floor;
            TileHit hit = gridManager.GetHitFromWorldPosition(mousePos, layer);

            if (hit.tile != null
                && hit.tile.IsGatherable) {
                Debug.Log("Color change");
                hit.tile.GatherInteraction(hit.gridPosition, layer);
            }
        }

        if (viewChanged) {
            cameraController.UpdateView();
        }
    }
    private Vector2Int MouseGridPosition(TileMapLayer buildingLayer) {
        Vector3 mousePos = GetcameraComp().ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return gridManager.WorldToGridPosition(mousePos, buildingLayer);
    }
    private Camera GetcameraComp() => cameraController.GetCurrentActiveCamera;
}
