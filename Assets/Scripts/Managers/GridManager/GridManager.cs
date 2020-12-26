using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BuildingLayer { Floor, Buildings }
public partial class GridManager : MonoBehaviour, IGridManager
{
    //Debug Chunks (Disable when not needed, very heavy on performance):
    #region Debug


    private void OnDrawGizmos() {
        foreach (Chunk chunk in chunksDict.Values) {
            Vector2Int minCorner = chunk.chunkStartPos;
            Vector2Int maxCorner = minCorner + Vector2Int.one * chunkSize;
            Vector3 leftCorner = GridToWorldPosition(Vector2Int.RoundToInt(new Vector2(minCorner.x, maxCorner.y)), BuildingLayer.Floor);
            Vector3 rightCorner = GridToWorldPosition(Vector2Int.RoundToInt(new Vector2(maxCorner.x, minCorner.y)), BuildingLayer.Floor);
            Vector3 topCorner = GridToWorldPosition(maxCorner, BuildingLayer.Floor);
            Vector3 bottomCorner = GridToWorldPosition(minCorner, BuildingLayer.Floor);
            //Debug.Log("Origin: Min:" + chunkStartCorner + ", Max:" + maxCorner + ", Real: Bottom:" + bottomCorner + ", Top:" + topCorner + "Left:" + leftCorner + ", Right:" + rightCorner);
            Gizmos.DrawLine(bottomCorner, leftCorner);
            Gizmos.DrawLine(bottomCorner, rightCorner);
            Gizmos.DrawLine(leftCorner, topCorner);
            Gizmos.DrawLine(rightCorner, topCorner);
        }
    }
    #endregion

    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Tilemap floor;
    [SerializeField]
    private Tilemap buildings;
    public Tilemap GetTilemap(BuildingLayer buildingLayer) {
        switch (buildingLayer) {
            case BuildingLayer.Floor:
                return floor;
            case BuildingLayer.Buildings:
                return buildings;
            default:
                return null;
        }
    }
    [SerializeField]
    private Noise islandsNoise;
    [SerializeField]
    int loadDistance;
    [SerializeField]
    float offSet;

    Vector2Int lastViewMin = Vector2Int.zero;
    Vector2Int lastViewMax = Vector2Int.zero;


    private const int chunkSize = 16;
    private const int collisionSensetivity = 6;
    private const float buildingLayerPositionOffset = 0.5f;
    public static GridManager _instance;

    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
        islandsNoise.GenerateSeed();
    }
    private void Start() {
        Init();
    }
    private void Init() {

    }

    public void UpdateView(Rect view) {

        Vector2Int bottomLeft = WorldToGridPosition(new Vector2(Mathf.Min(view.min.x, view.max.x), Mathf.Min(view.min.y, view.max.y)), BuildingLayer.Floor);
        Vector2Int topRight = WorldToGridPosition(new Vector2(Mathf.Max(view.min.x, view.max.x), Mathf.Max(view.min.y, view.max.y)), BuildingLayer.Floor);
        Vector2Int topLeft = WorldToGridPosition(new Vector2(Mathf.Min(view.min.x, view.max.x), Mathf.Max(view.min.y, view.max.y)), BuildingLayer.Floor);
        Vector2Int bottomRight = WorldToGridPosition(new Vector2(Mathf.Max(view.min.x, view.max.x), Mathf.Min(view.min.y, view.max.y)), BuildingLayer.Floor);
        Vector2Int min = new Vector2Int(bottomLeft.x, bottomRight.y) - Vector2Int.one * chunkSize * loadDistance;
        Vector2Int max = new Vector2Int(topRight.x, topLeft.y) + Vector2Int.one * chunkSize * ( loadDistance + 1 );
        min = GridToChunkCoordinates(min);
        max = GridToChunkCoordinates(max);
        if (lastViewMin == Vector2Int.zero && lastViewMax == Vector2Int.zero) {
            for (int loopX = min.x; loopX < max.x; loopX += chunkSize) {
                for (int loopY = min.y; loopY < max.y; loopY += chunkSize) {
                    Vector2Int currentPos = new Vector2Int(loopX, loopY);
                    if (!TryGetChunk(currentPos, out Chunk chunk)) {
                        CreateChunk(currentPos).GenerateIslands();
                    }
                }
            }
            lastViewMin = min;
            lastViewMax = max;
        }
        else {
            if (min != lastViewMin) {
                //Remove islands
                if (min.x > lastViewMin.x || min.y > lastViewMin.y) {

                    RemoveTiles(lastViewMin, new Vector2Int(min.x, max.y));
                    RemoveTiles(lastViewMin, new Vector2Int(max.x,min.y));
                }

                //Create new islands

                if (min.x < lastViewMin.x || min.y < lastViewMin.y) {

                    CreateTiles(min, new Vector2Int(lastViewMin.x, lastViewMax.y));
                    CreateTiles(min, new Vector2Int(lastViewMax.x,lastViewMin.y));
                }
                //Save view change
                lastViewMin = min;
            }
            if (max != lastViewMax) {
                //Remove islands
                if (max.x < lastViewMax.x || max.y < lastViewMax.y) {
                    RemoveTiles(new Vector2Int(min.x, max.y), lastViewMax);
                    RemoveTiles(new Vector2Int(max.x, min.y), lastViewMax);
                }

                //Create new islands
                
                if (max.x > lastViewMax.x || max.y > lastViewMax.y) {
                    CreateTiles(new Vector2Int(lastViewMin.x, lastViewMax.y), max);
                    CreateTiles(new Vector2Int(lastViewMax.x, lastViewMin.y), max);
                }
                //Save view change
                lastViewMax = max;
            }
        }


        void CreateTiles(Vector2Int from, Vector2Int to) {
            for (int loopX = from.x; loopX < to.x; loopX += chunkSize) {
                for (int loopY = from.y; loopY < to.y; loopY += chunkSize) {
                    Vector2Int currentPos = new Vector2Int(loopX, loopY);
                    if (!TryGetChunk(currentPos, out Chunk chunk)) {
                        CreateChunk(currentPos).GenerateIslands();
                    }
                }
            }
        }
        void RemoveTiles(Vector2Int from, Vector2Int to) {
            for (int loopX = from.x; loopX < to.x; loopX += chunkSize) {
                for (int loopY = from.y; loopY < to.y; loopY += chunkSize) {
                    Vector2Int currentPos = new Vector2Int(loopX, loopY);
                    if (TryGetChunk(currentPos, out Chunk chunk)) {
                        chunk.MarkOutOfView();
                    }
                }
            }
        }
    }
    public Vector3 GridToWorldPosition(Vector2Int gridPosition, BuildingLayer buildingLayer) => grid.CellToWorld((Vector3Int)gridPosition) + Vector3.up * (buildingLayer == BuildingLayer.Buildings ? buildingLayerPositionOffset : 0f);
    public Vector2Int WorldToGridPosition(Vector3 worldPosition, BuildingLayer buildingLayer)
        => (Vector2Int)GetTilemap(buildingLayer).WorldToCell(worldPosition - Vector3.up * (buildingLayer == BuildingLayer.Buildings ? buildingLayerPositionOffset : 0f));
    public bool IsTileWalkable(Vector2 worldPosition, Vector2 movementVector) {
        bool moveLegal = true;
        TileAbst floorTile = GetTileFromGrid(WorldToGridPosition(worldPosition + movementVector.normalized * offSet, BuildingLayer.Floor), BuildingLayer.Floor);
        moveLegal &= floorTile != null;
        Quaternion rotationLeft = Quaternion.Euler(0, 0, 90f / collisionSensetivity);
        Quaternion rotationRight = Quaternion.Euler(0, 0, 90f / collisionSensetivity);
        Vector2 leftMovementVector = movementVector.normalized * offSet;
        Vector2 rightMovementVector = movementVector.normalized * offSet;
        for (int i = 0; i < collisionSensetivity && moveLegal; i++) {
            leftMovementVector = rotationLeft * leftMovementVector;
            floorTile = GetTileFromGrid(WorldToGridPosition(worldPosition + leftMovementVector, BuildingLayer.Floor), BuildingLayer.Floor);
            moveLegal &= floorTile != null;
            rightMovementVector = rotationRight * rightMovementVector;
            floorTile = GetTileFromGrid(WorldToGridPosition(worldPosition + rightMovementVector, BuildingLayer.Floor), BuildingLayer.Floor);
            moveLegal &= floorTile != null;
        }
        return moveLegal;
    }
    public TileAbst GetTileFromGrid(Vector2Int gridPosition, BuildingLayer buildingLayer) {
        if (TryGetChunk(GridToChunkCoordinates(gridPosition), out Chunk chunk)) {
            TileAbst tile = chunk.GetTile(gridPosition, buildingLayer);
            return tile;
        }
        else {
            return null;
        }
    }
    public TileAbst GetTileFromWorld(Vector2 worldPosition, BuildingLayer buildingLayer) => GetTileFromGrid(WorldToGridPosition(worldPosition, buildingLayer), buildingLayer);

    /// <summary>
    /// Gets the tile on the at a certain position.
    /// Input grid position for an exact position on the grid
    /// and world position if you want to also check for tiles sides.
    /// </summary>
    /// <param name="clickPosition"></param>
    /// Position in world space in vector2
    /// <returns></returns>
    public TileHit GetHitFromClickPosition(Vector2 clickPosition, BuildingLayer buildingLayer) {
        Vector2Int gridPosition = WorldToGridPosition(clickPosition, buildingLayer);
        TileHit hit;
        if (TryGetChunk(GridToChunkCoordinates(gridPosition), out Chunk chunk)) {
            hit = new TileHit(chunk.GetTile(gridPosition, buildingLayer), gridPosition);
            if (hit.tile != null) {
                return hit;
            }
            else {
                Vector2 tileWorldPosition = GridToWorldPosition(gridPosition, buildingLayer);
                Vector2 localPosition = clickPosition - tileWorldPosition;
                if (localPosition.y < 0.2f && Mathf.Abs(localPosition.x) < 0.1f) {
                    return hit;
                }
                Vector2Int checkPosition = gridPosition + (localPosition.x < 0 ? Vector2Int.up : Vector2Int.right);
                hit = new TileHit(GetTileFromGrid(checkPosition, buildingLayer), checkPosition);
                if (hit.tile != null) {
                    return hit;
                }
                else if (Vector2.Distance(localPosition, new Vector2(0, 0.45f)) <= 0.24f) {
                    checkPosition = gridPosition + Vector2Int.one;
                    hit = new TileHit(GetTileFromGrid(checkPosition, buildingLayer), checkPosition);
                    if (hit.tile != null) {
                        return hit;
                    }
                }

            }

        }
        return TileHit.None;

    }
    public void SetTile(TileAbst tile, Vector2Int gridPosition, BuildingLayer buildingLayer, bool playerAction = true) {
        Vector2Int chunkPos = GridToChunkCoordinates(gridPosition);
        if (TryGetChunk(chunkPos, out Chunk chunk)) {
            chunk.SetTile(tile, gridPosition, buildingLayer, playerAction);
        }
        else if (tile != null) {
            CreateChunk(chunkPos).SetTile(tile, gridPosition, buildingLayer, playerAction);
        }
    }

    //Chunks Dictionary by chunk coordinates
    private static Dictionary<Vector2Int, Chunk> chunksDict = new Dictionary<Vector2Int, Chunk>();
    private Vector2Int GridToChunkCoordinates(Vector2Int gridPosition)
    => new Vector2Int(Mathf.FloorToInt((float)gridPosition.x / chunkSize) * chunkSize, Mathf.FloorToInt((float)gridPosition.y / chunkSize) * chunkSize);
    private bool TryGetChunk(Vector2Int chunkCoordinates, out Chunk chunk)
        => chunksDict.TryGetValue(chunkCoordinates, out chunk);
    private Chunk CreateChunk(Vector2Int chunkPosition) {
        Chunk chunk = new Chunk(chunkPosition);
        chunksDict.Add(chunkPosition, chunk);
        return chunk;
    }
}
