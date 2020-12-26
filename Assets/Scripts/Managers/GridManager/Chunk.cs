using System;
using UnityEngine;
using UnityEngine.Tilemaps;
public partial class GridManager
{
    //Chunk
    internal class Chunk
    {
        private TileAbst[,] floorArr = null;
        private TileAbst[,] buildingsArr = null;


        internal readonly Vector2Int chunkStartPos;
        private int tileCount = 0;
        private bool WasEdited = false;
        public Chunk(Vector2Int StartPos) {
            chunkStartPos = StartPos;
        }
        public TileAbst GetTile(Vector2Int gridPosition, BuildingLayer buildingLayer) {
            switch (buildingLayer) {
                case BuildingLayer.Floor:
                    return floorArr?[gridPosition.x - chunkStartPos.x, gridPosition.y - chunkStartPos.y];
                case BuildingLayer.Buildings:
                    return buildingsArr?[gridPosition.x - chunkStartPos.x, gridPosition.y - chunkStartPos.y];
                default:
                    throw new NotImplementedException();
            }

        }

        internal void SetTile(TileAbst tile, Vector2Int gridPosition, BuildingLayer buildingLayer, bool countsAsEdit = true) {
            switch (buildingLayer) {
                case BuildingLayer.Floor:
                    SetTileByRef(tile, gridPosition, buildingLayer, countsAsEdit, ref floorArr, ref _instance.floor);
                    break;
                case BuildingLayer.Buildings:
                    SetTileByRef(tile, gridPosition, buildingLayer, countsAsEdit, ref buildingsArr, ref _instance.buildings);
                    break;
            }

        }
        private void SetTileByRef(TileAbst tile, Vector2Int gridPosition, BuildingLayer buildingLayer, bool countsAsEdit, ref TileAbst[,] tileArr, ref Tilemap tilemap) {
            Vector2Int chunkPosition = GridToChunkPosition(gridPosition);
            bool tileExists = tileArr != null && tileArr[chunkPosition.x, chunkPosition.y] != null;
            if (tile != null || tileExists) {
                if (tileArr == null) {
                    tileArr = new TileAbst[chunkSize, chunkSize];
                }
                if (tileExists && tile == null) {
                    tileArr[chunkPosition.x, chunkPosition.y].Remove();
                    tilemap.SetTile((Vector3Int)gridPosition, null);
                    tileArr[chunkPosition.x, chunkPosition.y] = null;

                    tileCount--;
                    WasEdited |= countsAsEdit;
                    if (tileCount == 0) {
                        tileArr = null;
                    }

                }
                else if (!tileExists && tile != null) {
                    tileCount++;
                    WasEdited |= countsAsEdit;
                    tile.ImportVariables(gridPosition, buildingLayer);
                    tilemap.SetTile((Vector3Int)gridPosition, tile.mainTileBase);
                    tileArr[chunkPosition.x, chunkPosition.y] = tile;
                    tile.Init(gridPosition, buildingLayer);
                }
                else if (tile != tileArr[chunkPosition.x, chunkPosition.y]) {
                    tileArr[chunkPosition.x, chunkPosition.y].Remove();
                    tile.ImportVariables(gridPosition, buildingLayer);
                    tilemap.SetTile((Vector3Int)gridPosition, tile.mainTileBase);
                    tileArr[chunkPosition.x, chunkPosition.y] = tile;
                    tile.Init(gridPosition, buildingLayer);

                    WasEdited |= countsAsEdit;
                }
            }

        }
        internal Vector2Int GridToChunkPosition(Vector2Int gridPosition) => gridPosition - chunkStartPos;
        internal Vector2Int ChunkToGridPosition(Vector2Int chunkPosition) => chunkPosition + chunkStartPos;
        internal void GenerateIslands() {
            Noise noise = _instance.islandsNoise;
            for (int loopX = 0; loopX < chunkSize; loopX++) {
                for (int loopY = 0; loopY < chunkSize; loopY++) {
                    Vector2Int gridPosition = new Vector2Int(loopX, loopY) + chunkStartPos;
                    if (noise.CheckThreshold(gridPosition)) {
                        SetTile(new ObsidianTile(), ChunkToGridPosition(new Vector2Int(loopX, loopY)), BuildingLayer.Floor, false);
                    }
                }
            }
        }
        public void MarkOutOfView() {
            if (!WasEdited) {
                for (int loopX = 0; loopX < chunkSize; loopX++) {
                    for (int loopY = 0; loopY < chunkSize; loopY++) {
                        SetTile(null, ChunkToGridPosition(new Vector2Int(loopX, loopY)), BuildingLayer.Floor, false);
                    }
                }
                chunksDict.Remove(chunkStartPos);
            }
        }
    }
}
