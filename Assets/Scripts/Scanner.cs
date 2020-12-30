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
        private TileMapLayer buildingLayer;
        private IChecker checker;
        private DirectionEnum direction;

        public Scanner() {
            gridManager = GridManager._instance;
        }
<<<<<<< HEAD
        public TileHitStruct Scan(Vector2Int gridStartPosition, DirectionEnum direction, int radius, TileMapLayer buildingLayer, IChecker checker) {
=======
        public TileHitStruct Scan(Vector2Int gridStartPosition, DirectionEnum _direction, int _radius, TileMapLayer _buildingLayer, IChecker _checker) {

>>>>>>> master
            startPosition = gridStartPosition;
            radius = _radius;
            buildingLayer = _buildingLayer;
            checker = _checker;
            direction = _direction;
            for (int i = 1; i <= radius; i++) {
                TileHitStruct tileHit = GetClosestTileInSquare(i);
                if (tileHit.tile != null) {
                    return tileHit;
                }
            }

            return TileHitStruct.none;
        }

        public TileHitStruct GetClosestTileInSquare(int distanceFromCenter) {
            TileHitStruct[] tiles = GetAllTilesInSquare(distanceFromCenter);

            if (tiles == null) return TileHitStruct.none;

            TileHitStruct closestTile = TileHitStruct.none;
            float shortestDistance = float.MaxValue;
            foreach (TileHitStruct tile in tiles)
            {
                float distance;
                if ((distance = Vector2Int.Distance(tile.gridPosition, startPosition)) < shortestDistance) {
                    closestTile = tile;
                    shortestDistance = distance;

                }
            }

            return closestTile.tile != null ? closestTile : TileHitStruct.none;
        }
        private TileHitStruct[] GetAllTilesInSquare(int distanceFromCenter) {
            List<TileHitStruct> tiles = new List<TileHitStruct>();
            int numOfTiles = 8 * distanceFromCenter;
            Vector2Int relativeCheckPosition;
            switch (direction) {
                case DirectionEnum.Up:
<<<<<<< HEAD
                    relativeCheckPosition = new Vector2Int(0, 1);
                    break;
                case DirectionEnum.Down:
                    relativeCheckPosition = new Vector2Int(0, -1);
                    break;
                case DirectionEnum.Left:
                    relativeCheckPosition = new Vector2Int(-1, 0);
                    break;
                case DirectionEnum.Right:
                    relativeCheckPosition = new Vector2Int(1, 0);
                    break;
                default:
                    relativeCheckPosition = new Vector2Int(0, 1);
=======
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
>>>>>>> master
                    Debug.LogError("Added new not existing direction, this is not a 3d game...");
                    break;
            }

            

            DirectionEnum currentDirection = direction;
            for (int i = 0; i < numOfTiles; i++) {
                //Check tile

<<<<<<< HEAD
                TileAbst currentTile = gridManager.GetTileFromGrid(relativeCheckPosition + startPosition , buildingLayer);

                if (currentTile != null && checker.CheckTile(currentTile))
                {
                    tiles.Add(new TileHitStruct(currentTile, relativeCheckPosition));
                }
                //Check if at corner
                if (relativeCheckPosition.x == relativeCheckPosition.y) {
=======
                GenericTile currentTile = gridManager.GetTileFromGrid(relativeCheckPosition + startPosition , buildingLayer);

                if (currentTile != null && checker.CheckTile(currentTile)) {
                    tiles.Add(new TileHitStruct(currentTile, relativeCheckPosition + startPosition));
                }
                //Check if at corner
                if (Mathf.Abs(relativeCheckPosition.x) == Mathf.Abs(relativeCheckPosition.y)) {
>>>>>>> master
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
<<<<<<< HEAD
        bool CheckTile(TileAbst tile);
=======
        bool CheckTile(GenericTile tile);
>>>>>>> master
    }
}

