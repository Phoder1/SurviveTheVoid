using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BuildingLayers { Floor, Buildings}
public class GridManager : MonoBehaviour
{
    //Debug Chunks (Disable when not needed, very heavy on performance):
    #region Debug


    /*private void OnDrawGizmos() {
        foreach (Chunk chunk in chunksDict.Values) {
            Vector2Int minCorner = chunk.chunkStartPos;
            Vector2Int maxCorner = minCorner + Vector2Int.one * 16;
            Vector3 leftCorner = GridToWorldPosition(Vector2Int.RoundToInt(new Vector2(minCorner.x, maxCorner.y)));
            Vector3 rightCorner = GridToWorldPosition(Vector2Int.RoundToInt(new Vector2(maxCorner.x, minCorner.y)));
            Vector3 topCorner = GridToWorldPosition(maxCorner);
            Vector3 bottomCorner = GridToWorldPosition(minCorner);
            //Debug.Log("Origin: Min:" + chunkStartCorner + ", Max:" + maxCorner + ", Real: Bottom:" + bottomCorner + ", Top:" + topCorner + "Left:" + leftCorner + ", Right:" + rightCorner);
            Gizmos.DrawLine(bottomCorner, leftCorner);
            Gizmos.DrawLine(bottomCorner, rightCorner);
            Gizmos.DrawLine(leftCorner, topCorner);
            Gizmos.DrawLine(rightCorner, topCorner);
        }
    }*/
    #endregion

    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Tilemap floor;
    [SerializeField]
    private Tilemap buildings;
    [SerializeField]
    float noiseResolution;
    [Range(0f, 1f)]
    [SerializeField]
    float islandsThreshold;
    [SerializeField]
    int islandsAreaSize;
    [SerializeField]
    int loadDistance;

    Vector2Int lastViewMin = Vector2Int.zero;
    Vector2Int lastViewMax = Vector2Int.zero;


    private const int chunkSize = 16;
    private int seed;
    public static GridManager _instance;
    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
        seed = Random.Range(-10000, 10000);
    }


    private void Start() {
        Init();
    }
    private void Init() {

    }

    public void UpdateView(Rect view) {

        Vector2Int bottomLeft = WorldToGridPosition(new Vector2(Mathf.Min(view.min.x, view.max.x), Mathf.Min(view.min.y, view.max.y)));
        Vector2Int topRight = WorldToGridPosition(new Vector2(Mathf.Max(view.min.x, view.max.x), Mathf.Max(view.min.y, view.max.y)));
        Vector2Int topLeft = WorldToGridPosition(new Vector2(Mathf.Min(view.min.x, view.max.x), Mathf.Max(view.min.y, view.max.y)));
        Vector2Int bottomRight = WorldToGridPosition(new Vector2(Mathf.Max(view.min.x, view.max.x), Mathf.Min(view.min.y, view.max.y)));
        Vector2Int min = GridToChunkCoordinates(new Vector2Int(bottomLeft.x, bottomRight.y)) - Vector2Int.one * chunkSize * loadDistance;
        Vector2Int max = GridToChunkCoordinates(new Vector2Int(topRight.x, topLeft.y)) + Vector2Int.one * chunkSize * loadDistance;
        if(lastViewMin == Vector2Int.zero && lastViewMax == Vector2Int.zero) {
            Debug.Log("Min: " + min + ", Max: " + max);
            Debug.Log("Generating...");
            for (int loopX = min.x; loopX < max.x; loopX += chunkSize) {
                for (int loopY = min.y; loopY < max.y; loopY += chunkSize) {
                    Vector2Int currentPos = new Vector2Int(loopX, loopY);
                    if (!TryGetChunk(currentPos, out Chunk chunk)) {
                        CreateChunk(currentPos).GenerateIslands(seed, noiseResolution, islandsThreshold);
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
                    for (int loopX = lastViewMin.x; loopX <= min.x; loopX += chunkSize) {
                        for (int loopY = lastViewMin.y; loopY <= max.y; loopY += chunkSize) {
                            Vector2Int currentPos = new Vector2Int(loopX, loopY);
                            if (TryGetChunk(currentPos, out Chunk chunk)) {
                                chunk.MarkOutOfView();
                            }
                        }
                    }
                    for (int loopX = lastViewMin.x; loopX <= max.x; loopX += chunkSize) {
                        for (int loopY = lastViewMin.y; loopY <= min.y; loopY += chunkSize) {
                            Vector2Int currentPos = new Vector2Int(loopX, loopY);
                            if (TryGetChunk(currentPos, out Chunk chunk)) {
                                chunk.MarkOutOfView();
                            }
                        }
                    }
                }
                //Create new islands
                if (min.x < lastViewMin.x || min.y < lastViewMin.y) {
                    for (int loopX = min.x; loopX < lastViewMin.x; loopX += chunkSize) {
                        for (int loopY = min.y; loopY < max.y; loopY += chunkSize) {
                            Vector2Int currentPos = new Vector2Int(loopX, loopY);
                            if (!TryGetChunk(currentPos, out Chunk chunk)) {
                                CreateChunk(currentPos).GenerateIslands(seed, noiseResolution, islandsThreshold);
                            }
                        }
                    }
                    for (int loopX = min.x; loopX < max.x; loopX += chunkSize) {
                        for (int loopY = min.y; loopY < lastViewMax.y; loopY += chunkSize) {
                            Vector2Int currentPos = new Vector2Int(loopX, loopY);
                            if (!TryGetChunk(currentPos, out Chunk chunk)) {
                                CreateChunk(currentPos).GenerateIslands(seed, noiseResolution, islandsThreshold);
                            }
                        }
                    }
                }
                //Save view change
                lastViewMin = min;
            }
            if (max != lastViewMax) {
                //Remove islands
                if (max.x < lastViewMax.x || max.y < lastViewMax.y) {
                    for (int loopX = min.x; loopX <= lastViewMax.x; loopX += chunkSize) {
                        for (int loopY = max.y; loopY <= lastViewMax.y; loopY += chunkSize) {
                            Vector2Int currentPos = new Vector2Int(loopX, loopY);
                            if (TryGetChunk(currentPos, out Chunk chunk)) {
                                chunk.MarkOutOfView();
                            }
                        }
                    }
                    for (int loopX = max.x; loopX <= lastViewMax.x; loopX += chunkSize) {
                        for (int loopY = min.y; loopY <= lastViewMax.y; loopY += chunkSize) {
                            Vector2Int currentPos = new Vector2Int(loopX, loopY);
                            if (TryGetChunk(currentPos, out Chunk chunk)) {
                                chunk.MarkOutOfView();
                            }
                        }
                    }
                }

                //Create new islands
                if (max.x > lastViewMax.x || max.y > lastViewMax.y) {
                    for (int loopX = min.x; loopX < max.x; loopX += chunkSize) {
                        for (int loopY = lastViewMax.y; loopY < max.y; loopY += chunkSize) {
                            Vector2Int currentPos = new Vector2Int(loopX, loopY);
                            if (!TryGetChunk(currentPos, out Chunk chunk)) {
                                CreateChunk(currentPos).GenerateIslands(seed, noiseResolution, islandsThreshold);
                            }
                        }
                    }
                    for (int loopX = lastViewMax.x; loopX < max.x; loopX += chunkSize) {
                        for (int loopY = min.y; loopY < max.y; loopY += chunkSize) {
                            Vector2Int currentPos = new Vector2Int(loopX, loopY);
                            if (!TryGetChunk(currentPos, out Chunk chunk)) {
                                CreateChunk(currentPos).GenerateIslands(seed, noiseResolution, islandsThreshold);
                            }
                        }
                    }
                }
                //Save view change
                lastViewMax = max;
            }
        }
    }



    public Vector3 GridToWorldPosition(Vector2Int gridPosition) => grid.CellToWorld((Vector3Int)gridPosition);
    
    public Vector2Int WorldToGridPosition(Vector3 worldPosition) => (Vector2Int)grid.WorldToCell(worldPosition);
    public Tiles.TileAbst GetTile(Vector2Int gridPosition) {
        if (TryGetChunk(GridToChunkCoordinates(gridPosition), out Chunk chunk)) {
            return chunk.GetTile(gridPosition);
        }
        else {
            return null;
        }
    }
    public void SetTile(Vector2Int gridPosition, Tiles.TileAbst tile, bool playerAction = true) {
        Vector2Int chunkPos = GridToChunkCoordinates(gridPosition);
        if (TryGetChunk(chunkPos, out Chunk chunk)) {
            chunk.SetTile(gridPosition, tile, playerAction);
        }
        else if (tile != null) {
            CreateChunk(chunkPos).SetTile(gridPosition, tile, playerAction);
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

    //Chunk
    internal class Chunk
    {
        private Tiles.TileAbst[,] chunkArr = null;
        internal readonly Vector2Int chunkStartPos;
        private int tileCount = 0;
        private bool WasEdited = false;
        public Chunk(Vector2Int StartPos) {
            chunkStartPos = StartPos;
        }
        public Tiles.TileAbst GetTile(Vector2Int gridPosition) => chunkArr?[gridPosition.x - chunkStartPos.x, gridPosition.y - chunkStartPos.y];
        internal void SetTile(Vector2Int gridPosition, Tiles.TileAbst tile, bool countsAsEdit = true) {
            Vector2Int chunkPosition = GridToChunkPosition(gridPosition);
            bool tileExists = chunkArr != null && chunkArr[chunkPosition.x, chunkPosition.y] != null;
            if (tile != null || tileExists) {
                if (chunkArr == null) {
                    chunkArr = new Tiles.TileAbst[chunkSize, chunkSize];
                }
                if (tileExists && tile == null) {
                    chunkArr[chunkPosition.x, chunkPosition.y].Remove();
                    _instance.floor.SetTile((Vector3Int)gridPosition, null);
                    chunkArr[chunkPosition.x, chunkPosition.y] = null;

                    tileCount--;
                    WasEdited |= countsAsEdit;
                    if (tileCount == 0) {
                        chunkArr = null;
                    }

                }
                else if (!tileExists && tile != null) {
                    tileCount++;
                    WasEdited |= countsAsEdit;

                    _instance.floor.SetTile((Vector3Int)gridPosition, tile.tileBase);
                    chunkArr[chunkPosition.x, chunkPosition.y] = tile;
                    tile.Init(gridPosition);
                }
                else if(tile != chunkArr[chunkPosition.x, chunkPosition.y]) {
                    chunkArr[chunkPosition.x, chunkPosition.y].Remove();
                    _instance.floor.SetTile((Vector3Int)gridPosition, tile.tileBase);
                    chunkArr[chunkPosition.x, chunkPosition.y] = tile;
                    tile.Init(gridPosition);

                    WasEdited |= countsAsEdit;
                }
            }

        }
        internal Vector2Int GridToChunkPosition(Vector2Int gridPosition) => gridPosition - chunkStartPos;
        internal Vector2Int ChunkToGridPosition(Vector2Int chunkPosition) => chunkPosition + chunkStartPos;
        internal void GenerateIslands(int seed, float noiseResolution, float islandsThreshold) {
            for (int loopX = 0; loopX < chunkSize; loopX++) {
                for (int loopY = 0; loopY < chunkSize; loopY++) {
                    float perlinNoise = Mathf.PerlinNoise((float)(loopX + seed + chunkStartPos.x) / noiseResolution, (float)(loopY + seed + chunkStartPos.y) / noiseResolution);
                    if (perlinNoise > islandsThreshold) {
                        SetTile(ChunkToGridPosition(new Vector2Int(loopX, loopY)), new Tiles.ToothPaste(), false);
                    }
                }

            }
        }
        public void MarkOutOfView() {
            if (!WasEdited) {
                for (int loopX = 0; loopX < chunkSize; loopX++) {
                    for (int loopY = 0; loopY < chunkSize; loopY++) {
                        SetTile(ChunkToGridPosition(new Vector2Int(loopX, loopY)), null, false);
                    }
                }
                chunksDict.Remove(chunkStartPos);
            }
        }
    }
}
