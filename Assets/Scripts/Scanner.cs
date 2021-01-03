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
        private TileMapLayer buildingLayer;
        private IChecker checker;
        private DirectionEnum direction;

        public Scanner() {
            gridManager = GridManager._instance;
        }

        public TileHit Scan(Vector2Int gridStartPosition, DirectionEnum _direction, int _radius, TileMapLayer _buildingLayer, IChecker _checker) {
            startPosition = gridStartPosition;
            radius = _radius;
            buildingLayer = _buildingLayer;
            checker = _checker;
            direction = _direction;
            for (int i = 1; i <= radius; i++) {
                TileHit tileHit = GetClosestTileInSquare(i);
                if (tileHit != null) {
                    return tileHit;
                }
            }

            return null;
        }

        public TileHit GetClosestTileInSquare(int distanceFromCenter) {
            TileHit[] tiles = GetAllTilesInSquare(distanceFromCenter);

            if (tiles == null) return null;

            TileHit closestTile = null;
            float shortestDistance = float.MaxValue;
            foreach (TileHit tile in tiles)
            {
                float distance;
                if ((distance = Vector2Int.Distance(tile.gridPosition, startPosition)) < shortestDistance) {
                    closestTile = tile;
                    shortestDistance = distance;

                }
            }

            return closestTile != null ? closestTile : null;
        }
        private TileHit[] GetAllTilesInSquare(int distanceFromCenter) {
            List<TileHit> tiles = new List<TileHit>();
            int numOfTiles = 8 * distanceFromCenter;
            Vector2Int relativeCheckPosition;
            switch (direction) {
                case DirectionEnum.Up:
                    relativeCheckPosition = new Vector2Int(0, 1) * distanceFromCenter;
                    break;
                case DirectionEnum.Down:
                    relativeCheckPosition = new Vector2Int(0, -1) * distanceFromCenter;
                    break;
                case DirectionEnum.Left:
                    relativeCheckPosition = new Vector2Int(-1, 0) * distanceFromCenter;
                    break;
                case DirectionEnum.Right:
                    relativeCheckPosition = new Vector2Int(1, 0) * distanceFromCenter;
                    break;
                default:
                    relativeCheckPosition = new Vector2Int(0, 1) * distanceFromCenter;
                    Debug.LogError("Added new not existing direction, this is not a 3d game...");
                    break;
            }

            

            DirectionEnum currentDirection = direction;
            for (int i = 0; i < numOfTiles; i++) {
                //Check tile
                TileSlot currentTile = gridManager.GetTileFromGrid(relativeCheckPosition + startPosition , buildingLayer);

                if (currentTile != null && checker.CheckTile(currentTile))
                {
                    tiles.Add(new TileHit(currentTile, relativeCheckPosition));
                }
                //Check if at corner
                if (relativeCheckPosition.x == relativeCheckPosition.y) {
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
                        relativeCheckPosition += new Vector2Int(1, 0);
                        break;
                    case DirectionEnum.Down:
                        relativeCheckPosition += new Vector2Int(-1, 0);
                        break;
                    case DirectionEnum.Left:
                        relativeCheckPosition += new Vector2Int(0, 1);
                        break;
                    case DirectionEnum.Right:
                        relativeCheckPosition += new Vector2Int(0, -1);
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
        bool CheckTile(TileSlot tile);
    }
}

