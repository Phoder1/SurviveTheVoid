using UnityEngine;

public class PlayerMovementHandler : MonoSingleton<PlayerMovementHandler>
{
    [Range(0.1f, 1f)]
    [SerializeField] float playerColliderSize;
    CameraController cameraController;
    GridManager gridManager;
    PlayerManager playerManager;


    Vector2Int currentGridPos;
    Vector2 gridMoveVector;

    private float tileRealSize = Mathf.Sin(Mathf.Atan(Mathf.Deg2Rad * 0.5f));
    private Vector2 realGridAxisVector;
    DirectionEnum worldMovementDirection;
    public override void Init() {
        playerManager = PlayerManager._instance;
        cameraController = CameraController._instance;
        gridManager = GridManager._instance;
        Debug.Log(GridToUnityVector(new Vector2(1, 1)));
    }
    public void Move(Vector2 moveVector) {
        gridMoveVector = UnityToGridVector(moveVector);
        currentGridPos = gridManager.WorldToGridPosition(transform.position, TileMapLayer.Floor);
        MoveOnY();
        MoveOnX();
    }

    private void MoveOnY() {
        Vector2 UnityVectorOnGridY = GridToUnityVector(new Vector2(0, gridMoveVector.y));
        if(UnityVectorOnGridY == Vector2.zero) {
            return;
        }
        if (gridMoveVector.y > 0) {
            if (CheckTilesOnPos(tileLeftCorner(transform.position) + UnityVectorOnGridY) && CheckTilesOnPos(tileTopCorner(transform.position) + UnityVectorOnGridY)) {
                transform.position += (Vector3)UnityVectorOnGridY;
            }
            //else {
            //    Vector2 nextGridPos = UnityToGridVector(transform.position + (Vector3)UnityVectorOnGridY);
            //    nextGridPos.y = Mathf.Floor(nextGridPos.y);
            //    transform.position = (Vector3)GridToUnityVector(nextGridPos) + Vector3.forward * transform.position.z;
            //    ;
            //}
        }
        else {
            if (CheckTilesOnPos(tileBottomCorner(transform.position) + UnityVectorOnGridY) && CheckTilesOnPos(tileRightCorner(transform.position) + UnityVectorOnGridY)) {
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
            if (CheckTilesOnPos(tileLeftCorner(transform.position) + UnityVectorOnGridX) && CheckTilesOnPos(tileTopCorner(transform.position) + UnityVectorOnGridX)) {
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
            if (CheckTilesOnPos(tileBottomCorner(transform.position) + UnityVectorOnGridX) && CheckTilesOnPos(tileRightCorner(transform.position) + UnityVectorOnGridX)) {
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
    private Vector2 tileTopCorner(Vector2 pos) => pos + Vector2.up * 0.25f * playerColliderSize;
    private Vector2 tileBottomCorner(Vector2 pos) => pos + Vector2.down * 0.25f * playerColliderSize;
    private Vector2 tileRightCorner(Vector2 pos) => pos + Vector2.right * 0.5f * playerColliderSize;
    private Vector2 tileLeftCorner(Vector2 pos) => pos + Vector2.left * 0.5f * playerColliderSize;
    private void UpdateView() => cameraController.UpdateView();
    //public bool IsTileWalkable(Vector2 worldPosition, Vector2 movementVector) {
    //    if (movementVector == Vector2.zero || movementVector.magnitude < 0.01f)
    //        return true;
    //    bool moveLegal = true;
    //    TileSlot floorTile = GetTileFromGrid(WorldToGridPosition(worldPosition + movementVector + movementVector.normalized * floorOffSet, TileMapLayer.Floor), TileMapLayer.Floor);
    //    TileSlot buildingTile = GetTileFromGrid(WorldToGridPosition(worldPosition + movementVector + movementVector.normalized * buildingsOffSet, TileMapLayer.Buildings), TileMapLayer.Buildings);
    //    moveLegal &= floorTile != null && !(buildingTile != null && buildingTile.GetIsSolid);
    //    if (!moveLegal)
    //        return moveLegal;
    //    Quaternion rotationLeft = Quaternion.Euler(0, 0, 75f / COLLISION_SENSITIVITY);
    //    Quaternion rotationRight = Quaternion.Euler(0, 0, -75f / COLLISION_SENSITIVITY);
    //    Vector2 leftMovementVector = movementVector + movementVector.normalized * floorOffSet;
    //    Vector2 rightMovementVector = movementVector + movementVector.normalized * floorOffSet;
    //    for (int i = 0; i < COLLISION_SENSITIVITY && moveLegal; i++) {
    //        leftMovementVector = rotationLeft * leftMovementVector;
    //        floorTile = GetTileFromGrid(WorldToGridPosition(worldPosition + leftMovementVector, TileMapLayer.Floor), TileMapLayer.Floor);
    //        buildingTile = GetTileFromGrid(WorldToGridPosition(worldPosition + leftMovementVector, TileMapLayer.Buildings), TileMapLayer.Buildings);
    //        moveLegal &= floorTile != null && !(buildingTile != null && buildingTile.GetIsSolid);
    //        rightMovementVector = rotationRight * rightMovementVector;
    //        floorTile = GetTileFromGrid(WorldToGridPosition(worldPosition + rightMovementVector, TileMapLayer.Floor), TileMapLayer.Floor);
    //        buildingTile = GetTileFromGrid(WorldToGridPosition(worldPosition + leftMovementVector, TileMapLayer.Buildings), TileMapLayer.Buildings);
    //        moveLegal &= floorTile != null && !(buildingTile != null && buildingTile.GetIsSolid);
    //    }
    //    return moveLegal;
    //}
    private void UpdateWorldDirection(Vector2 moveVector) {
        float angle = Vector2.SignedAngle(moveVector, Vector2.up);
        int direction = Mathf.RoundToInt(angle / 90);
        switch (direction) {
            case 0:
                worldMovementDirection = DirectionEnum.Up;
                break;
            case 1:
                worldMovementDirection = DirectionEnum.Right;
                break;
            case -1:
                worldMovementDirection = DirectionEnum.Left;
                break;
            case 2:
            case -2:
                worldMovementDirection = DirectionEnum.Down;
                break;
            default:
                worldMovementDirection = DirectionEnum.Down;
                break;
        }
    }
}
