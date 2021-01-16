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
                    throw new System.NotImplementedException();
            }

        }
        internal void SetTile(TileSlot tile, Vector2Int gridPosition, TileMapLayer buildingLayer, bool countsAsEdit = true, bool generation = false) {

            switch (buildingLayer) {
                case TileMapLayer.Floor:
                    SetTileByRef(tile, gridPosition, buildingLayer, countsAsEdit, ref floorArr, ref _instance.floor, generation);
                    break;
                case TileMapLayer.Buildings:
                    SetTileByRef(tile, gridPosition, buildingLayer, countsAsEdit, ref buildingsArr, ref _instance.buildings, generation);
                    break;
            }

        }
        private void SetTileByRef(TileSlot tile, Vector2Int gridPosition, TileMapLayer tilemapLayer, bool countsAsEdit, ref TileSlot[,] tileArr, ref Tilemap tilemap, bool generation = false) {

            Vector2Int chunkPosition = GridToChunkPosition(gridPosition);
            bool tileExists = tileArr != null && tileArr[chunkPosition.x, chunkPosition.y] != null;
            if (tile != null || tileExists) {
                if (tileArr == null) {
                    tileArr = new TileSlot[CHUNK_SIZE, CHUNK_SIZE];
                }
                if (tileExists && tile == null) {
                    tileArr[chunkPosition.x, chunkPosition.y].CancelEvent(gridPosition, tilemapLayer);
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
                    tile.Init(gridPosition, tilemapLayer, generation);
                    tilemap.SetTile((Vector3Int)gridPosition, tile.GetMainTileBase);
                    tileArr[chunkPosition.x, chunkPosition.y] = tile;
                }
                else {
                    if (tile != tileArr[chunkPosition.x, chunkPosition.y]) {
                        tileArr[chunkPosition.x, chunkPosition.y].CancelEvent(gridPosition, tilemapLayer);
                        tileArr[chunkPosition.x, chunkPosition.y] = tile;
                        tile.Init(gridPosition, tilemapLayer, generation);
                    }
                    tilemap.SetTile((Vector3Int)gridPosition, tile.GetMainTileBase);
                    wasEdited |= countsAsEdit;
                }
            }
        }
        internal Vector2Int GridToChunkPosition(Vector2Int gridPosition) => gridPosition - chunkStartPos;
        internal Vector2Int ChunkToGridPosition(Vector2Int chunkPosition) => chunkPosition + chunkStartPos;
        internal void GenerateIslands() {
            Noise islandsNoise = _instance.islandsNoise;
            GridRandom buildingsRandom = _instance.buildingsRandom;
            TileTierStruct[] tiers = _instance.floorBlocksTiers;
            BuildingGenStruct[] buildings = _instance.buildingsGeneration;
            for (int loopX = 0; loopX < CHUNK_SIZE; loopX++) {
                for (int loopY = 0; loopY < CHUNK_SIZE; loopY++) {
                    Vector2Int gridPosition = new Vector2Int(loopX, loopY) + chunkStartPos;
                    float distance = Vector2Int.Distance(gridPosition, Vector2Int.zero);
                    if (distance > _instance.clearZoneRadius) {
                        if (islandsNoise.CheckThreshold(gridPosition, out float noiseValue)) {
                            TileAbstSO tile = tiers[tiers.Length - 1].tile;
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
                            SetTile(new TileSlot(tile), ChunkToGridPosition(new Vector2Int(loopX, loopY)), TileMapLayer.Floor, false, true);
                            TileAbstSO building = ChooseBuilding(gridPosition, distance);
                            if (building != null) {
                                SetTile(new TileSlot(building), gridPosition, TileMapLayer.Buildings, false, true);
                            }
                        }
                    }
                    else if (distance <= _instance.startIslandRadius) {
                        SetTile(new TileSlot(_instance.startIslandTile), ChunkToGridPosition(new Vector2Int(loopX, loopY)), TileMapLayer.Floor, false, true);
                    }
                }
            }
            TileAbstSO ChooseBuilding(Vector2Int gridPosition, float distance) {
                float sumWeight = 0f;
                float[] chances = new float[buildings.Length];
                for (int i = 0; i < buildings.Length; i++) {
                    if (buildings[i].global) {
                        chances[i] = buildings[i].chance;
                    }
                    else {
                    chances[i] = easeInOutSine(1- Mathf.Clamp(Mathf.Abs(buildings[i].distance - distance) / buildings[i].spread, 0f, 1f)) * buildings[i].chance;
                    }
                    sumWeight += chances[i];
                }
                if (sumWeight > 1) {
                    for (int i = 0; i < chances.Length; i++) {
                        chances[i] /= sumWeight;
                    }
                }
                float roll = buildingsRandom.GetRandomValue(gridPosition);
                for (int i = 0; i < chances.Length; i++) {
                    roll -= chances[i];
                    if (roll <= 0f) {
                        return buildings[i].tile;
                    }
                }
                return null;

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



        private float easeInOutSine(float x) => -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
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


