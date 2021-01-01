using System;
using UnityEngine;
using UnityEngine.Tilemaps;
public partial class GridManager
{
    //Chunk
    internal class Chunk
    {
        private TileSlot[,] floorArr;
        private TileSlot[,] buildingsArr;


        internal readonly Vector2Int chunkStartPos;
        private int tileCount;
        private bool wasEdited;
        public Chunk(Vector2Int startPos) {
            chunkStartPos = startPos;
        }
        public TileSlot GetTile(Vector2Int gridPosition, TileMapLayer buildingLayer) {
            switch (buildingLayer) {
                case TileMapLayer.Floor:
                    return floorArr?[gridPosition.x - chunkStartPos.x, gridPosition.y - chunkStartPos.y];
                case TileMapLayer.Buildings:
                    return buildingsArr?[gridPosition.x - chunkStartPos.x, gridPosition.y - chunkStartPos.y];
                default:
                    throw new NotImplementedException();
            }

        }
        internal void SetTile(TileSlot tile, Vector2Int gridPosition, TileMapLayer buildingLayer, bool countsAsEdit = true) {

            switch (buildingLayer) {
                case TileMapLayer.Floor:
                    SetTileByRef(tile, gridPosition, buildingLayer, countsAsEdit, ref floorArr, ref _instance.floor);
                    break;
                case TileMapLayer.Buildings:
                    SetTileByRef(tile, gridPosition, buildingLayer, countsAsEdit, ref buildingsArr, ref _instance.buildings);
                    break;
            }

        }
        private void SetTileByRef(TileSlot tile, Vector2Int gridPosition, TileMapLayer buildingLayer, bool countsAsEdit, ref TileSlot[,] tileArr, ref Tilemap tilemap) {

            Vector2Int chunkPosition = GridToChunkPosition(gridPosition);
            bool tileExists = tileArr != null && tileArr[chunkPosition.x, chunkPosition.y] != null;
            if (tile != null || tileExists) {
                if (tileArr == null) {
                    tileArr = new TileSlot[CHUNK_SIZE, CHUNK_SIZE];
                }
                if (tileExists && tile == null) {
                    tileArr[chunkPosition.x, chunkPosition.y].Remove(gridPosition, buildingLayer);
                    tilemap.SetTile((Vector3Int)gridPosition, null);
                    tileArr[chunkPosition.x, chunkPosition.y] = null;

                    tileCount--;
                    wasEdited |= countsAsEdit;
                    if (tileCount == 0) {
                        tileArr = null;
                    }

                }
                else if (!tileExists) {
                    tileCount++;
                    wasEdited |= countsAsEdit;
                    tilemap.SetTile((Vector3Int)gridPosition, tile.GetMainTileBase);
                    tileArr[chunkPosition.x, chunkPosition.y] = tile;
                }
                else if (tile != tileArr[chunkPosition.x, chunkPosition.y]) {
                    tileArr[chunkPosition.x, chunkPosition.y].Remove(gridPosition, buildingLayer);
                    tilemap.SetTile((Vector3Int)gridPosition, tile.GetMainTileBase);
                    tileArr[chunkPosition.x, chunkPosition.y] = tile;

                    wasEdited |= countsAsEdit;
                }
            }

        }
        internal Vector2Int GridToChunkPosition(Vector2Int gridPosition) => gridPosition - chunkStartPos;
        internal Vector2Int ChunkToGridPosition(Vector2Int chunkPosition) => chunkPosition + chunkStartPos;
        internal void GenerateIslands() {
            Noise islandsNoise = _instance.islandsNoise;
            Noise plantsNoise = _instance.plantsNoise;
            TileTier[] tiers = _instance.floorBlocksTiers;
            TileAbstSO plant = _instance.plantTile;
            for (int loopX = 0; loopX < CHUNK_SIZE; loopX++) {
                for (int loopY = 0; loopY < CHUNK_SIZE; loopY++) {
                    Vector2Int gridPosition = new Vector2Int(loopX, loopY) + chunkStartPos;
                    if (islandsNoise.CheckThreshold(gridPosition, true, out float noiseValue)) {
                        TileAbstSO tile = tiers[tiers.Length-1].tile;
                        float distance = Vector2Int.Distance(gridPosition, Vector2Int.zero);
                        for (int i = 0; i < tiers.Length; i++) {
                            if (distance <= tiers[i].distance) {
    
                                float overlap = (distance - tiers[i].overlapStart) / (tiers[i].distance - tiers[i].overlapStart);
                                if (distance > tiers[i].overlapStart && easeInOutBounce(overlap) > islandsNoise.GetRandomValue(gridPosition)) {
                                    tile = tiers[i].tile;
                                }
                                else {
                                    tile = tiers[i - 1].tile;
                                }
                                break;
                            }
                            
                            
                        }
                        SetTile(new TileSlot(tile), ChunkToGridPosition(new Vector2Int(loopX, loopY)), TileMapLayer.Floor, false);
                        if (plantsNoise.CheckThreshold(gridPosition, false, out _)) {
                            SetTile(new TileSlot(plant), gridPosition, TileMapLayer.Buildings, false);
                        }
                    }
                }
            }
        }
        public void MarkOutOfView() {
            if (!wasEdited) {
                for (int loopX = 0; loopX < CHUNK_SIZE; loopX++) {
                    for (int loopY = 0; loopY < CHUNK_SIZE; loopY++) {
                        SetTile(null, ChunkToGridPosition(new Vector2Int(loopX, loopY)), TileMapLayer.Floor, false);
                    }
                }
                for (int loopX = 0; loopX < CHUNK_SIZE; loopX++) {
                    for (int loopY = 0; loopY < CHUNK_SIZE; loopY++) {
                        SetTile(null, ChunkToGridPosition(new Vector2Int(loopX, loopY)), TileMapLayer.Buildings, false);
                    }
                }
                chunksDict.Remove(chunkStartPos);
            }
        }

        private float EaseInOutQuart(float x) => -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
        private float easeOutBounce(float x) {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (x < 1 / d1) {
                return n1 * x * x;
            }
            else if (x < 2 / d1) {
                return n1 * (x -= 1.5f / d1) * x + 0.75f;
            }
            else if (x < 2.5 / d1) {
                return n1 * (x -= 2.25f / d1) * x + 0.9375f;
            }
            else {
                return n1 * (x -= 2.625f / d1) * x + 0.984375f;
            }

        }
        private float easeInBounce(float x) => 1 - easeOutBounce(1 - x);

        private float easeInOutBounce(float x) => x < 0.5f ? (1 - easeOutBounce(1 - 2 * x)) / 2 : (1 + easeOutBounce(2 * x - 1)) / 2;
    }
}


