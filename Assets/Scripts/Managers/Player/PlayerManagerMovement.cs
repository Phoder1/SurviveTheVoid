using Assets.Scan;
using System.Collections;
using UnityEngine;
public partial class PlayerManager
{
    [Range(0.1f, 1f)]
    [SerializeField] float playerColliderSize;
    CameraController cameraController;

    Vector2Int currentGridPos;
    Vector2 gridMoveVector;
    [Min(0.1f)]
    [SerializeField] float animationSpeedMultiplier;
    Vector2 totalSpeed;

    bool moved;
    public void Move(Vector2 moveVector) {
        moved = false;
        gridMoveVector = UnityToGridVector(moveVector);
        currentGridPos = gridManager.WorldToGridPosition((Vector2)transform.position, TileMapLayer.Floor);
        if (Input.GetKey(KeyCode.LeftShift)) {
            moved = true;
            transform.Translate(moveVector);
        }
        else {
            totalSpeed = Vector2.zero;
            MoveOnY();
            MoveOnX();
            totalSpeed.y *= 2;
        }
        if (moved)
            UpdateView();
        Debug.Log("Left corner:" + gridManager.WorldToGridPosition(tileLeftCorner(transform.position, playerColliderSize), TileMapLayer.Floor)
            + " ,Top corner:" + gridManager.WorldToGridPosition(tileLeftCorner(transform.position, playerColliderSize), TileMapLayer.Floor)
            + " ,Right corner:" + gridManager.WorldToGridPosition(tileRightCorner(transform.position, playerColliderSize), TileMapLayer.Floor)
            + " ,Bottom corner:" + gridManager.WorldToGridPosition(tileBottomCorner(transform.position, playerColliderSize), TileMapLayer.Floor)
            );
    }

    private void MoveOnY() {
        Vector2 UnityVectorOnGridY = GridToUnityVector(new Vector2(0, gridMoveVector.y));
        if (UnityVectorOnGridY == Vector2.zero) {
            return;
        }
        if (gridMoveVector.y > 0) {
            if (CheckTilesOnPos(tileLeftCorner(transform.position, playerColliderSize) + UnityVectorOnGridY) && CheckTilesOnPos(tileTopCorner(transform.position, playerColliderSize) + UnityVectorOnGridY)) {
                totalSpeed += UnityVectorOnGridY;
                ApplyMove(UnityVectorOnGridY);
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
                totalSpeed += UnityVectorOnGridY;
                ApplyMove(UnityVectorOnGridY);
            }
            //else {
            //    Vector2 nextGridPos = UnityToGridVector(transform.position + (Vector3)UnityVectorOnGridY);
            //    nextGridPos.y = Mathf.Ceil(nextGridPos.y);
            //    transform.position = (Vector3)GridToUnityVector(nextGridPos) + Vector3.forward * transform.position.z;
            //}
        }
    }
    private void MoveOnX() {
        Vector2 UnityVectorOnGridX = GridToUnityVector(new Vector2(gridMoveVector.x, 0));
        if (UnityVectorOnGridX == Vector2.zero) {
            return;
        }
        if (gridMoveVector.x > 0) {
            if (CheckTilesOnPos(tileRightCorner(transform.position, playerColliderSize) + UnityVectorOnGridX) && CheckTilesOnPos(tileTopCorner(transform.position, playerColliderSize) + UnityVectorOnGridX)) {
                totalSpeed += UnityVectorOnGridX;
                ApplyMove(UnityVectorOnGridX);
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
                totalSpeed += UnityVectorOnGridX;
                ApplyMove(UnityVectorOnGridX);
            }
            //else {
            //    Vector2 nextGridPos = UnityToGridVector(transform.position + (Vector3)UnityVectorOnGridX);
            //    nextGridPos.x = Mathf.Ceil(nextGridPos.x);
            //    transform.position = (Vector3)GridToUnityVector(nextGridPos) + Vector3.forward * transform.position.z;
            //}
        }
    }
    private void ApplyMove(Vector2 vector) => ApplyMove((Vector3)vector);
    private void ApplyMove(Vector3 vector) {
        transform.position += vector;
        moved = true;
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

