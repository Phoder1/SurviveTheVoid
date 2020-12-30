﻿using Assets.TilesData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileMapLayer { Floor, Buildings }
public partial class GridManager : MonoBehaviour, IGridManager
{
    //Debug Chunks (Disable when not needed, very heavy on performance):
    #region Debug


    private void OnDrawGizmos() {
        foreach (Chunk chunk in chunksDict.Values) {
            Vector2Int minCorner = chunk.chunkStartPos;
            Vector2Int maxCorner = minCorner + Vector2Int.one * CHUNK_SIZE;
            Vector3 leftCorner = GridToWorldPosition(Vector2Int.RoundToInt(new Vector2(minCorner.x, maxCorner.y)), TileMapLayer.Floor, false);
            Vector3 rightCorner = GridToWorldPosition(Vector2Int.RoundToInt(new Vector2(maxCorner.x, minCorner.y)), TileMapLayer.Floor, false);
            Vector3 topCorner = GridToWorldPosition(maxCorner, TileMapLayer.Floor, false);
            Vector3 bottomCorner = GridToWorldPosition(minCorner, TileMapLayer.Floor, false);
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
    public Tilemap GetTilemap(TileMapLayer buildingLayer) {
        switch (buildingLayer) {
            case TileMapLayer.Floor:
                return floor;
            case TileMapLayer.Buildings:
                return buildings;
            default:
                return null;
        }
    }
    [SerializeField] private Noise islandsNoise;
    [SerializeField] private Noise plantsNoise;
    [SerializeField] private int loadDistance;
    [SerializeField] private float offSet;
    public TilesPackSO tilesPack;

    Vector2Int lastViewMin = Vector2Int.zero;
    Vector2Int lastViewMax = Vector2Int.zero;

    private TileTier[] floorBlocksTiers;
    private protected readonly struct TileTier
    {
        public readonly TileAbst tile;
        public readonly float distance;
        public readonly float overlapStart;

        public TileTier(TileAbst tile, float distance, float overlapStart) {
            this.tile = tile;
            this.distance = distance;
            this.overlapStart = overlapStart;
        }
    }

    private const int CHUNK_SIZE = 16;
    private const int COLLISION_SENSETIVITY = 6;
    private const float BUILDING_LAYER_POSITION_OFFSET = 0.5f;
    public const float TILE_CAP_HEIGHT = 0.7f;
    
    
    public static GridManager _instance;

    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
        islandsNoise.GenerateSeed();
        plantsNoise.GenerateSeed();
        Init();
    }

    
    private void Init() {
        floorBlocksTiers = new TileTier[3] {
        new TileTier(_instance.tilesPack.GetMoonTile,0f,0f),
        new TileTier(_instance.tilesPack.GetObsidianTile,1000f,0f),
        new TileTier(_instance.tilesPack.GetCircusTile,2000f,1000f)
    };
    }

    public void UpdateView(Rect view) {

        Vector2Int bottomLeft = WorldToGridPosition(new Vector2(Mathf.Min(view.min.x, view.max.x), Mathf.Min(view.min.y, view.max.y)), TileMapLayer.Floor);
        Vector2Int topRight = WorldToGridPosition(new Vector2(Mathf.Max(view.min.x, view.max.x), Mathf.Max(view.min.y, view.max.y)), TileMapLayer.Floor);
        Vector2Int topLeft = WorldToGridPosition(new Vector2(Mathf.Min(view.min.x, view.max.x), Mathf.Max(view.min.y, view.max.y)), TileMapLayer.Floor);
        Vector2Int bottomRight = WorldToGridPosition(new Vector2(Mathf.Max(view.min.x, view.max.x), Mathf.Min(view.min.y, view.max.y)), TileMapLayer.Floor);
        Vector2Int min = new Vector2Int(bottomLeft.x, bottomRight.y) - Vector2Int.one * CHUNK_SIZE * loadDistance;
        Vector2Int max = new Vector2Int(topRight.x, topLeft.y) + Vector2Int.one * CHUNK_SIZE * (loadDistance + 1);
        min = GridToChunkCoordinates(min);
        max = GridToChunkCoordinates(max);
        if (lastViewMin == Vector2Int.zero && lastViewMax == Vector2Int.zero) {
            for (int loopX = min.x; loopX < max.x; loopX += CHUNK_SIZE) {
                for (int loopY = min.y; loopY < max.y; loopY += CHUNK_SIZE) {
                    Vector2Int currentPos = new Vector2Int(loopX, loopY);

                    if (!TryGetChunk(currentPos, out _)) {
                        CreateChunk(currentPos).GenerateIslands();
                    }
                }
            }
            lastViewMin = min;
            lastViewMax = max;
        }
        else {
            if (min != lastViewMin) {
                //Create new islands

                if (min.x < lastViewMin.x) {
                    CreateTiles(min, new Vector2Int(lastViewMin.x, max.y));
                }
                if (min.y < lastViewMin.y) {
                    CreateTiles(min, new Vector2Int(max.x, lastViewMin.y));
                }
                //Remove islands
                if (min.x > lastViewMin.x) {
                    RemoveTiles(lastViewMin, new Vector2Int(min.x, max.y + 1));
                }
                if (min.y > lastViewMin.y) {
                    RemoveTiles(lastViewMin, new Vector2Int(max.x + 1, min.y));
                }
                //Save view change
                lastViewMin = min;
            }
            if (max != lastViewMax) {
                //Create new islands

                if (max.x > lastViewMax.x) {
                    CreateTiles(new Vector2Int(lastViewMax.x, min.y), max);
                }
                if (max.y > lastViewMax.y) {
                    CreateTiles(new Vector2Int(min.x, lastViewMax.y), max);
                }
                //Remove islands
                if (max.x < lastViewMax.x) {
                    RemoveTiles(new Vector2Int(max.x, min.y), lastViewMax + Vector2Int.one);
                }
                if (max.y < lastViewMax.y) {
                    RemoveTiles(new Vector2Int(min.x, max.y), lastViewMax + Vector2Int.one);
                }
                //Save view change
                lastViewMax = max;
            }
        }


        void CreateTiles(Vector2Int from, Vector2Int to) {
            for (int loopX = from.x; loopX < to.x; loopX += CHUNK_SIZE) {
                for (int loopY = from.y; loopY < to.y; loopY += CHUNK_SIZE) {
                    Vector2Int currentPos = new Vector2Int(loopX, loopY);
                    if (!TryGetChunk(currentPos, out Chunk chunk)) {
                        CreateChunk(currentPos).GenerateIslands();
                    }
                }
            }
        }
        void RemoveTiles(Vector2Int from, Vector2Int to) {
            for (int loopX = from.x; loopX < to.x; loopX += CHUNK_SIZE) {
                for (int loopY = from.y; loopY < to.y; loopY += CHUNK_SIZE) {
                    Vector2Int currentPos = new Vector2Int(loopX, loopY);
                    if (TryGetChunk(currentPos, out Chunk chunk)) {
                        chunk.MarkOutOfView();
                    }
                }
            }
        }
    }
    public Vector3 GridToWorldPosition(Vector2Int gridPosition, TileMapLayer buildingLayer, bool getCenter) 
        => grid.CellToWorld((Vector3Int)gridPosition) + Vector3.up * ((buildingLayer == TileMapLayer.Buildings ? BUILDING_LAYER_POSITION_OFFSET : 0f) + (getCenter?TILE_CAP_HEIGHT/2:0f));
    public Vector2Int WorldToGridPosition(Vector3 worldPosition, TileMapLayer buildingLayer)
        => (Vector2Int)GetTilemap(buildingLayer).WorldToCell(worldPosition - Vector3.up * (buildingLayer == TileMapLayer.Buildings ? BUILDING_LAYER_POSITION_OFFSET : 0f));
    public bool IsTileWalkable(Vector2 worldPosition, Vector2 movementVector) {
        bool moveLegal = true;
        TileAbst floorTile = GetTileFromGrid(WorldToGridPosition(worldPosition + movementVector.normalized * offSet, TileMapLayer.Floor), TileMapLayer.Floor);
        moveLegal &= floorTile != null;
        Quaternion rotationLeft = Quaternion.Euler(0, 0, 90f / COLLISION_SENSETIVITY);
        Quaternion rotationRight = Quaternion.Euler(0, 0, 90f / COLLISION_SENSETIVITY);
        Vector2 leftMovementVector = movementVector.normalized * offSet;
        Vector2 rightMovementVector = movementVector.normalized * offSet;
        for (int i = 0; i < COLLISION_SENSETIVITY && moveLegal; i++) {
            leftMovementVector = rotationLeft * leftMovementVector;
            floorTile = GetTileFromGrid(WorldToGridPosition(worldPosition + leftMovementVector, TileMapLayer.Floor), TileMapLayer.Floor);
            moveLegal &= floorTile != null;
            rightMovementVector = rotationRight * rightMovementVector;
            floorTile = GetTileFromGrid(WorldToGridPosition(worldPosition + rightMovementVector, TileMapLayer.Floor), TileMapLayer.Floor);
            moveLegal &= floorTile != null;
        }
        return moveLegal;
    }
    public TileAbst GetTileFromGrid(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        if (TryGetChunk(GridToChunkCoordinates(gridPosition), out Chunk chunk)) {
            TileAbst tile = chunk.GetTile(gridPosition, buildingLayer);
            return tile;
        }
        else {
            return null;
        }
    }
    public TileAbst GetTileFromWorld(Vector2 worldPosition, TileMapLayer buildingLayer) => GetTileFromGrid(WorldToGridPosition(worldPosition, buildingLayer), buildingLayer);

    /// <summary>
    /// Gets the tile on the at a certain position.
    /// Input grid position for an exact position on the grid
    /// and world position if you want to also check for tiles sides.
    /// </summary>
    /// <param name="clickPosition"></param>
    /// Position in world space in vector2
    /// <returns></returns>
    public TileHitStruct GetHitFromClickPosition(Vector2 clickPosition, TileMapLayer buildingLayer) {
        Vector2Int gridPosition = WorldToGridPosition(clickPosition, buildingLayer);
        TileHitStruct hit;
        if (TryGetChunk(GridToChunkCoordinates(gridPosition), out Chunk chunk)) {
            hit = new TileHitStruct(chunk.GetTile(gridPosition, buildingLayer), gridPosition);
            if (hit.tile != null) {
                return hit;
            }
            else {
                Vector2 tileWorldPosition = GridToWorldPosition(gridPosition, buildingLayer, true);
                Vector2 localPosition = clickPosition - tileWorldPosition;
                if (localPosition.y < 0.2f && Mathf.Abs(localPosition.x) < 0.1f) {
                    return hit;
                }
                Vector2Int checkPosition = gridPosition + (localPosition.x < 0 ? Vector2Int.up : Vector2Int.right);
                hit = new TileHitStruct(GetTileFromGrid(checkPosition, buildingLayer), checkPosition);
                if (hit.tile != null) {
                    return hit;
                }
                else if (Vector2.Distance(localPosition, new Vector2(0, 0.45f)) <= 0.24f) {
                    checkPosition = gridPosition + Vector2Int.one;
                    hit = new TileHitStruct(GetTileFromGrid(checkPosition, buildingLayer), checkPosition);
                    if (hit.tile != null) {
                        return hit;
                    }
                }

            }

        }
        return TileHitStruct.none;

    }
    public void SetTile(TileAbst tile, Vector2Int gridPosition, TileMapLayer buildingLayer, bool playerAction = true) {
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
    => new Vector2Int(Mathf.FloorToInt((float)gridPosition.x / CHUNK_SIZE) * CHUNK_SIZE, Mathf.FloorToInt((float)gridPosition.y / CHUNK_SIZE) * CHUNK_SIZE);
    private bool TryGetChunk(Vector2Int chunkCoordinates, out Chunk chunk)
        => chunksDict.TryGetValue(chunkCoordinates, out chunk);
    private Chunk CreateChunk(Vector2Int chunkPosition) {
        Chunk chunk = new Chunk(chunkPosition);
        chunksDict.Add(chunkPosition, chunk);
        return chunk;
    }
}
