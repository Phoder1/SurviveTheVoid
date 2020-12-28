using Assets.TilesData;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scan
{
    public enum DirectionEnum { Up, Down, Left, Right }
    public class Scanner
    {
        private readonly GridManager gridManager;
        private Vector2Int startPosition;
        private int radius;
        private BuildingLayer buildingLayer;
        private IChecker checker;
        private DirectionEnum direction;

        public Scanner() {
            if (gridManager == null)
                gridManager = GridManager._instance;
        }
        public TileHitStruct Scan(Vector2Int gridStartPosition, DirectionEnum direction, int radius, BuildingLayer buildingLayer, IChecker checker) {

            startPosition = gridStartPosition;
            this.radius = radius;
            this.buildingLayer = buildingLayer;
            this.checker = checker;
            this.direction = direction;
            for (int i = 1; i <= this.radius; i++)
            {
                TileHitStruct tile = GetClosestTileInSquare(i);
                if (tile.tile != null)
                {
                    return tile;
                }
            }

            return TileHitStruct.none;
        }

        public TileHitStruct GetClosestTileInSquare(int distanceFromCenter)
        {
            TileHitStruct[] tiles = GetAllTilesInSquare(distanceFromCenter);
            if (tiles != null)
            {
                TileHitStruct closestTile = TileHitStruct.none;
                float shortestDistance = float.MaxValue;
                foreach (TileHitStruct tile in tiles) {
                    if (Vector2Int.Distance(tile.gridPosition, startPosition) < shortestDistance)
                    {
                        closestTile = tile;
                    }
                }

                if (closestTile.tile != null)
                {
                    return closestTile;
                }
            }

            return TileHitStruct.none;

        }
        private TileHitStruct[] GetAllTilesInSquare(int distanceFromCenter)
        {
            List<TileHitStruct> tiles = new List<TileHitStruct>();
            int numOfTiles = 8 * distanceFromCenter;
            Vector2Int currentPosition;
            switch (direction) {
                case DirectionEnum.Up:
                    currentPosition = new Vector2Int(0, 1);
                    break;
                case DirectionEnum.Down:
                    currentPosition = new Vector2Int(0, -1);
                    break;
                case DirectionEnum.Left:
                    currentPosition = new Vector2Int(-1, 0);
                    break;
                case DirectionEnum.Right:
                    currentPosition = new Vector2Int(1, 0);
                    break;
                default:
                    currentPosition = new Vector2Int(0, 1);
                    Debug.LogError("Added new not existing direction, this is not a 3d game...");
                    break;
            }
            DirectionEnum currentDirection = direction;
            for (int i = 0; i < numOfTiles; i++) {

                //Check tile

                GenericTile currentTile = gridManager.GetTileFromGrid(currentPosition, buildingLayer);

                if (currentTile != null && checker.CheckTile(currentTile))
                {
                    tiles.Add(new TileHitStruct(currentTile, currentPosition));
                }
                //Check if at corner
                if (currentPosition.x == currentPosition.y) {
                    switch (currentDirection) {
                        case DirectionEnum.Up:
                            currentDirection = DirectionEnum.Right;
                            break;
                        case DirectionEnum.Down:
                            currentDirection = DirectionEnum.Left;
                            break;
                        case DirectionEnum.Left:
                            currentDirection = DirectionEnum.Up;
                            break;
                        case DirectionEnum.Right:
                            currentDirection = DirectionEnum.Down;
                            break;
                    }
                }

                //Update position
                switch (currentDirection) {
                    case DirectionEnum.Up:
                        currentPosition += new Vector2Int(1, 0);
                        break;
                    case DirectionEnum.Down:
                        currentPosition += new Vector2Int(-1, 0);
                        break;
                    case DirectionEnum.Left:
                        currentPosition += new Vector2Int(0, 1);
                        break;
                    case DirectionEnum.Right:
                        currentPosition += new Vector2Int(0, -1);
                        break;
                }
            }
            return tiles.ToArray();
        }


    }
    /// <summary>
    /// IChecker is a interface to contain a bool function that decides whether to take into account a certain element.
    /// </summary>
    public interface IChecker
    {
        /// <summary>
        /// Decides whether the script should take into account the referenced tile.
        /// </summary>
        /// <param name="tile">
        /// The tile to check, return true to take the tile into account.
        /// </param>
        /// <returns></returns>
        bool CheckTile(GenericTile tile);
    }
}

