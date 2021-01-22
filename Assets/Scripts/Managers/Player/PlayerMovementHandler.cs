using UnityEngine;

public class PlayerMovementHandler : MonoSingleton<PlayerMovementHandler>
{
    [Range(0.1f, 1f)]
    [SerializeField] float playerColliderSize;
    CameraController cameraController;
    GridManager gridManager;


    Vector2Int currentGridPos;
    Vector2 gridMoveVector;

    private float tileRealSize = Mathf.Sin(Mathf.Atan(Mathf.Deg2Rad * 0.5f));
    private Vector2 realGridAxisVector;
    DirectionEnum worldMovementDirection;
    public override void Init() {
        cameraController = CameraController._instance;
        gridManager = GridManager._instance;
        Debug.Log(GridToUnityVector(new Vector2(1, 1)));
    }
    public void Move(Vector2 moveVector) {
        gridMoveVector = UnityToGridVector(moveVector);
        currentGridPos = gridManager.WorldToGridPosition((Vector2)transform.position, TileMapLayer.Floor);
        MoveOnY();
        MoveOnX();
    }

    private void MoveOnY() {
        Vector2 UnityVectorOnGridY = GridToUnityVector(new Vector2(0, gridMoveVector.y));
        if(UnityVectorOnGridY == Vector2.zero) {
            return;
        }
        if (gridMoveVector.y > 0) {
            if (CheckTilesOnPos(tileLeftCorner(transform.position, playerColliderSize) + UnityVectorOnGridY) && CheckTilesOnPos(tileTopCorner(transform.position, playerColliderSize) + UnityVectorOnGridY)) {
                transform.position += (Vector3)UnityVectorOnGridY;
            }
            //else {
            //    Vector2 nextTilePos = gridManager.GridToWorldPosition(currentGridPos + Vector2Int.up, TileMapLayer.Floor, true);
            //    float gridDistance = UnityToGridVector(nextTilePos).y - UnityToGridVector(tileLeftCorner(transform.position, playerColliderSize)).y;
            //    Debug.Log(gridDistance);
            //    Vector2 moveVector = GridToUnityVector(new Vector2(0, gridDistance - 0.5f));
            //    transform.position += (Vector3)moveVector;
            //}
        }
        else {
            if (CheckTilesOnPos(tileBottomCorner(transform.position, playerColliderSize) + UnityVectorOnGridY) && CheckTilesOnPos(tileRightCorner(transform.position, playerColliderSize) + UnityVectorOnGridY)) {
                transform.position += (Vector3)UnityVectorOnGridY;
            }
            //else {
            //    Vector2 nextGridPos = UnityToGridVector(transform.position + (Vector3)UnityVectorOnGridY);
            //    nextGridPos.y = Mathf.Ceil(nextGridPos.y);
            //    transform.position = (Vector3)GridToUnityVector(nextGridPos) + Vector3.forward * transform.position.z;
            //}
        }
    }
    private void MoveOnX() {
        Vector2 UnityVectorOnGridX = GridToUnityVector(new Vector2( gridMoveVector.x, 0));
        if (UnityVectorOnGridX == Vector2.zero) {
            return;
        }
        if (gridMoveVector.x > 0) {
            if (CheckTilesOnPos(tileRightCorner(transform.position, playerColliderSize) + UnityVectorOnGridX) && CheckTilesOnPos(tileTopCorner(transform.position, playerColliderSize) + UnityVectorOnGridX)) {
                transform.position += (Vector3)UnityVectorOnGridX;
            }
            //else {
            //    Vector2 nextGridPos = UnityToGridVector(transform.position + (Vector3)UnityVectorOnGridX);
            //    nextGridPos.x = Mathf.Floor(nextGridPos.x);
            //    transform.position = (Vector3)GridToUnityVector(nextGridPos) + Vector3.forward * transform.position.z;
            //    ;
            //}
        }
        else {
            if (CheckTilesOnPos(tileBottomCorner(transform.position, playerColliderSize) + UnityVectorOnGridX) && CheckTilesOnPos(tileLeftCorner(transform.position, playerColliderSize) + UnityVectorOnGridX)) {
                transform.position += (Vector3)UnityVectorOnGridX;
            }
            //else {
            //    Vector2 nextGridPos = UnityToGridVector(transform.position + (Vector3)UnityVectorOnGridX);
            //    nextGridPos.x = Mathf.Ceil(nextGridPos.x);
            //    transform.position = (Vector3)GridToUnityVector(nextGridPos) + Vector3.forward * transform.position.z;
            //}
        }
    }

    private Vector2 UnityToGridVector(Vector2 vector) => new Vector2(2 * vector.x + vector.y, -2 * vector.x + vector.y);
    private Vector2 GridToUnityVector(Vector2 vector) => new Vector2(0.125f * vector.x - 0.125f * vector.y, 0.25f * vector.x + 0.25f * vector.y);
    private bool CheckTilesOnPos(Vector2 pos) {
        Vector2Int gridPos = gridManager.WorldToGridPosition(pos, TileMapLayer.Floor);
        if (gridPos == currentGridPos)
            return true;
        TileSlot buildingTile = gridManager.GetTileFromGrid(gridPos, TileMapLayer.Buildings);
        TileSlot floorTile = gridManager.GetTileFromGrid(gridPos, TileMapLayer.Floor);
        return (buildingTile == null || !buildingTile.GetIsSolid) && floorTile != null;
    }
    private Vector2 tileTopCorner(Vector2 pos, float colliderSize) => pos + Vector2.up * 0.25f * colliderSize;
    private Vector2 tileBottomCorner(Vector2 pos, float colliderSize) => pos + Vector2.down * 0.25f * colliderSize;
    private Vector2 tileRightCorner(Vector2 pos, float colliderSize) => pos + Vector2.right * 0.5f * colliderSize;
    private Vector2 tileLeftCorner(Vector2 pos, float colliderSize) => pos + Vector2.left * 0.5f * colliderSize;
    private void UpdateView() => cameraController.UpdateView();
}
