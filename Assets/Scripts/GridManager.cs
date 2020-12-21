using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BuildingLayers { Floor, Buildings}
public class GridManager : MonoBehaviour
{
    //Debug Chunks (Disable when not needed, very heavy on performance):
    #region Debug


    private void OnDrawGizmos() {
        foreach (Chunk chunk in chunksDict.Values) {
            Vector2Int minCorner = chunk.gridStartPos;
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
    }
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


    private const int chunkSize = 16;

    private Tiles tiles;
    public static GridManager _instance;
    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
    }


    private void Start() {
        tiles = Tiles._instance;
        Init();
        CreateIslands();
    }

    private void Init() {
    }

    //Perlin Noise
    private void CreateIslands() {
        Vector2Int startPosition = new Vector2Int(-islandsAreaSize, -islandsAreaSize);
        Vector2Int endPosition = new Vector2Int(islandsAreaSize, islandsAreaSize);
        Vector2Int size = endPosition - startPosition;
        float randomness = Random.Range(0, 5000);
        for (int loopX = startPosition.x; loopX < endPosition.x; loopX++) {
            for (int loopY = startPosition.y; loopY < endPosition.y; loopY++) {
                if (Vector2Int.Distance(new Vector2Int(loopX, loopY), Vector2Int.zero) < islandsAreaSize) {
                    float perlinNoise = Mathf.PerlinNoise((((float)(loopX - startPosition.x) / size.x + randomness) * noiseResolution), ((float)(loopY - startPosition.y) / size.y + randomness) * noiseResolution);
                    if (perlinNoise > islandsThreshold) {
                        SetTile(new Vector2Int(loopX, loopY), new Tiles.ToothPaste(), false);
                    }
                }
            }
        }
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition) => grid.CellToWorld((Vector3Int)gridPosition);
    
    public Vector2Int WorldToGridPosition(Vector3 gridPosition) => (Vector2Int)grid.WorldToCell(gridPosition);
    public Tiles.TileAbst GetTile(Vector2Int gridPosition) {
        if (TryGetChunk(GridToChunkPosition(gridPosition), out Chunk chunk)) {
            return chunk.GetTile(gridPosition);
        }
        else {
            return null;
        }
    }
    public void SetTile(Vector2Int gridPosition, Tiles.TileAbst tile, bool playerAction = true) {
        Vector2Int chunkPos = GridToChunkPosition(gridPosition);
        if (TryGetChunk(chunkPos, out Chunk chunk)) {
            chunk.SetTile(gridPosition, tile, playerAction);
        }
        else if (tile != null) {
            CreateChunk(chunkPos).SetTile(gridPosition, tile, playerAction);
        }
    }

    //Chunks Dictionary by chunk coordinates
    private static Dictionary<Vector2Int, Chunk> chunksDict = new Dictionary<Vector2Int, Chunk>();
    private Vector2Int GridToChunkPosition(Vector2Int gridPosition)
    => new Vector2Int(Mathf.FloorToInt((float)gridPosition.x / chunkSize) * chunkSize, Mathf.FloorToInt((float)gridPosition.y / chunkSize) * chunkSize);
    private bool TryGetChunk(Vector2Int chunkPosition, out Chunk chunk)
        => chunksDict.TryGetValue(chunkPosition, out chunk);
    private Chunk CreateChunk(Vector2Int chunkPosition) {
        Chunk chunk = new Chunk(chunkPosition);
        chunksDict.Add(chunkPosition, chunk);
        return chunk;
    }


    internal class Chunk
    {
        private Tiles.TileAbst[,] chunkArr = null;
        internal readonly Vector2Int gridStartPos;
        private int tileCount = 0;
        private bool chunkWasEdited = false;
        public Chunk(Vector2Int StartPos) {
            gridStartPos = StartPos;
        }
        public Tiles.TileAbst GetTile(Vector2Int gridPosition) => chunkArr?[gridPosition.x - gridStartPos.x, gridPosition.y - gridStartPos.y];
        


        /// <param name="gridPosition">
        /// A Vector2Int position on the grid.
        /// </param>
        internal void SetTile(Vector2Int gridPosition, Tiles.TileAbst tile, bool countsAsEdit = true) {
            Vector2Int chunkPos = gridPosition - gridStartPos;
            bool tileExists = chunkArr != null && chunkArr[chunkPos.x, chunkPos.y] != null;
            if (tile != null || tileExists) {
                if (chunkArr == null) {
                    chunkArr = new Tiles.TileAbst[chunkSize, chunkSize];
                }
                if (tileExists && tile == null) {
                    chunkArr[chunkPos.x, chunkPos.y].Remove();
                    _instance.floor.SetTile((Vector3Int)gridPosition, null);
                    chunkArr[chunkPos.x, chunkPos.y] = null;

                    tileCount--;
                    chunkWasEdited |= countsAsEdit;
                    if (tileCount == 0) {
                        chunkArr = null;

                        chunksDict.Remove(gridStartPos);
                    }

                }
                else if (!tileExists && tile != null) {
                    tileCount++;
                    chunkWasEdited |= countsAsEdit;

                    _instance.floor.SetTile((Vector3Int)gridPosition, tile.tileBase);
                    chunkArr[chunkPos.x, chunkPos.y] = tile;
                    tile.Init(gridPosition);
                }
                else if(tile != chunkArr[chunkPos.x, chunkPos.y]) {
                    chunkArr[chunkPos.x, chunkPos.y].Remove();
                    _instance.floor.SetTile((Vector3Int)gridPosition, tile.tileBase);
                    chunkArr[chunkPos.x, chunkPos.y] = tile;
                    tile.Init(gridPosition);

                    chunkWasEdited |= countsAsEdit;
                }
            }

        }
    }
}
