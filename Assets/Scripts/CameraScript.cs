using UnityEngine;

public class CameraScript : MonoSingleton<CameraScript>
{
    [SerializeField]
    private float scrollSpeed;
    [SerializeField]
    private float moveSpeed;
    private Vector2 movement;
    private float scrollMovement;
    private Camera cameraComp;
    private GridManager gridManager => GridManager._instance;
    private Vector2 cameraRealSize => new Vector2(GetCameraComp.orthographicSize * 2 * GetCameraComp.aspect, GetCameraComp.orthographicSize * 2);
    private Vector3 camCornerPosition => new Vector3(transform.position.x, transform.position.y, 0) - (Vector3)cameraRealSize / 2;
    private Rect worldView => new Rect(_instance.camCornerPosition, _instance.cameraRealSize);

    public Camera GetCameraComp {
        get {
            if (cameraComp == null)
                cameraComp = Camera.main;
            return cameraComp;
        }
    }

    private bool viewChanged;
    [SerializeField] private BlockTileSO clickTile;
    StarsParallaxController starsCont;
    // Start is called before the first frame update
    public override void Init() {
        starsCont = GetComponent<StarsParallaxController>();
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
            GetCameraComp.orthographicSize += scrollMovement;
            viewChanged = true;
            starsCont.UpdateViewSize();
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
            Vector3 mousePos = GetCameraComp.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            TileMapLayer layer = (Input.GetKey(KeyCode.LeftShift)) ? TileMapLayer.Buildings : TileMapLayer.Floor;
            TileHit hit = gridManager.GetHitFromWorldPosition(mousePos, layer);

            if (hit != null
                && hit.tile.GetInteractionType == InteractionType.Any) {
                Debug.Log("Color change");
                hit.tile.GatherInteraction(hit.gridPosition, layer);
            }
        }

        if (viewChanged) {
            UpdateView();
        }
    }
    private Vector2Int MouseGridPosition(TileMapLayer buildingLayer) {
        Vector3 mousePos = GetCameraComp.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return gridManager.WorldToGridPosition(mousePos, buildingLayer);
    }
    public void UpdateView() {
        gridManager.UpdateView(worldView);
    }
}
