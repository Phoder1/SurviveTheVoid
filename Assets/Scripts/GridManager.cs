using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    /*
    private void OnDrawGizmos() {
        foreach (ChunksGrid.Chunk chunk in chunksGrid.chunksDict.Values) {
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


    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Tilemap[] tilemapLayersArr;

    private Tiles tiles = Tiles._instance;

    public static GridManager _instance;

    private ChunksGrid chunksGrid = ChunksGrid._getInstance;
    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
    }

    public void SetTile(Vector2Int gridPosition, TilemapLayers layer, TileBase tile) {

    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition) => grid.CellToWorld((Vector3Int)gridPosition);
    public TileInterface GetTileAtPos(Vector2Int gridPosition) => chunksGrid.GetTile(gridPosition);
    public void SetTileAtPos(Vector2Int gridPosition, TileInterface tile) => chunksGrid.SetTile(gridPosition, tile);

    private class ChunksGrid
    {

        private const int chunkSize = 16;

        //Chunks Dictionary by chunk cordinates
        internal Dictionary<Vector2Int, Chunk> chunksDict;

        //Singleton
        private static ChunksGrid _instance;
        internal static ChunksGrid _getInstance {
            get {
                if (_instance == null) {
                    _instance = new ChunksGrid();
                }
                return _instance;
            }
        }

        //Initializer
        private ChunksGrid() => chunksDict = new Dictionary<Vector2Int, Chunk>();

        /// <param name="gridPosition">
        /// A Vector2Int position on the grid.
        /// </param>
        internal TileInterface GetTile(Vector2Int gridPosition) {
            if (TryGetChunk(GridToChunkPosition(gridPosition), out Chunk chunk)) {
                return chunk.GetTile(gridPosition);
            }
            else {
                return null;
            }
        }

        /// <param name="gridPosition">
        /// A Vector2Int position on the grid.
        /// </param>
        internal void SetTile(Vector2Int gridPosition, TileInterface tile) {
            Vector2Int chunkPos = GridToChunkPosition(gridPosition);
            if (TryGetChunk(chunkPos, out Chunk chunk)) {
                chunk.SetTile(gridPosition, tile);
            }
            else if (tile != null) {
                CreateChunk(chunkPos).SetTile(gridPosition, tile);
            }
        }

        /// <summary>Returns whether the chunk exists, and if it does, sent to out.</summary>
        /// <param name="chunkPosition">
        /// A Vector2Int position of the chunk.
        /// </param>
        private bool TryGetChunk(Vector2Int chunkPosition, out Chunk chunk)
            => chunksDict.TryGetValue(chunkPosition, out chunk);

        /// <param name="gridPosition">
        /// A Vector2Int position of the chunk.
        /// </param>
        private Chunk CreateChunk(Vector2Int chunkPosition) {
            Chunk chunk = new Chunk(chunkPosition);
            chunksDict.Add(chunkPosition, chunk);
            return chunk;
        }


        /// <param name="gridPosition">
        /// A Vector2Int position on the grid.
        /// </param>
        private Vector2Int GridToChunkPosition(Vector2Int gridPosition)
            => new Vector2Int(Mathf.FloorToInt((float)gridPosition.x / chunkSize) * chunkSize, Mathf.FloorToInt((float)gridPosition.y / chunkSize) * chunkSize);

        internal class Chunk
        {
            private TileInterface[,] chunkArr;
            internal readonly Vector2Int gridStartPos;
            private int tileCount = 0;
            private Vector2Int GridEndPos => gridStartPos + Vector2Int.one * chunkSize;
            public Chunk(Vector2Int StartPos) {
                gridStartPos = StartPos;
                chunkArr = new TileInterface[chunkSize, chunkSize];
            }

            /// <param name="gridPosition">
            /// A Vector2Int position on the grid.
            /// </param>
            internal TileInterface GetTile(Vector2Int gridPosition)
                => chunkArr[gridPosition.x - gridStartPos.x, gridPosition.y - gridStartPos.y];

            /// <param name="gridPosition">
            /// A Vector2Int position on the grid.
            /// </param>
            internal void SetTile(Vector2Int gridPosition, TileInterface tile) {
                Vector2Int chunkPos = gridPosition - gridStartPos;
                if (chunkArr[chunkPos.x, chunkPos.y] == null && tile != null) { tileCount++; }
                else if (chunkArr[chunkPos.x, chunkPos.y] != null && tile == null) {
                    tileCount--;
                    if (tileCount == 0) {
                        _getInstance.chunksDict.Remove(gridStartPos);
                    }
                }
                chunkArr[chunkPos.x, chunkPos.y] = tile;
                if (tile != null) {
                }
            }
        }
    }
    */
}
