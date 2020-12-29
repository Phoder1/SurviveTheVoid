using Assets.TimeEvents;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.TilesData
{
    public enum ToolInteractionEnum { None, Axe, Pickaxe, Hoe, Shovel, Hammer, Any }
    #region Tile hit
    public readonly struct TileHitStruct
    {
        public readonly Vector2Int gridPosition;
        public readonly TileAbst tile;

        public TileHitStruct(TileAbst tile, Vector2Int gridPosition) {
            this.tile = tile;
            this.gridPosition = gridPosition;
        }

        public static TileHitStruct none => new TileHitStruct(null, Vector2Int.zero);

        public static bool operator ==(TileHitStruct lhs, TileHitStruct rhs) {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(TileHitStruct lhs, TileHitStruct rhs) {
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object obj) {
            return (obj is TileHitStruct hit &&
                    EqualityComparer<Vector2Int?>.Default.Equals(gridPosition, hit.gridPosition) &&
                    EqualityComparer<TileAbst>.Default.Equals(tile, hit.tile));
        }
        public bool Equals(TileHitStruct other) {
            return tile == other.tile && gridPosition == other.gridPosition;
        }

        public override int GetHashCode() {
            int hashCode = 1814505039;
            hashCode = hashCode * -1521134295 + gridPosition.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TileAbst>.Default.GetHashCode(tile);
            return hashCode;
        }
    }
    #endregion
    #region Main abstract class
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public abstract class TileAbst
    {
        public TileBase mainTileBase;
        public ToolInteractionEnum interactionType;
        public bool isActiveInteraction;

        public bool isSolid;
        public virtual void Init(Vector2Int _position, TileMapLayer buildingLayer) { }
        private protected TileAbst() { }

        public virtual TileAbst Clone() {
            TileAbst copy = (TileAbst)MemberwiseClone();
            copy.mainTileBase = mainTileBase;
            return copy;

        }
        public virtual void Remove() {

        }
        public virtual void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) { }
        public virtual void LongPressUpdate() { }

        [Serializable]
        public struct TileVariations
        {
            public TileBase tileBase;
            public float chanceWeight;
        }
    }
    #endregion

    #region Subtypes
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class BlockTile : TileAbst
    {

    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class GatherableTile : TileAbst
    {
        [SerializeField] ItemSlot[] rewards;
        public override void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
            GridManager._instance.SetTile(null, gridPosition, buildingLayer, true);
            Inventory inventory = Inventory.GetInstance;
            foreach (ItemSlot reward in rewards) {
                inventory.AddToInventory(reward);
            }
            inventory.PrintInventory();

        }
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class TimedEventTiles : TileAbst
    {
        protected TimeEvent eventInstance;
        public override void Remove() {
            base.Remove();
            if (eventInstance != null)
                eventInstance.Cancel();
        }
    }
    #endregion
    #region Actual Tiles
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class ToothPaste : TimedEventTiles
    {
        public TileAbst replacementTile;
        public int eventDelay;
        private ToothPaste() { }
        public override TileAbst Clone() {
            ToothPaste copy = (ToothPaste)base.Clone();
            copy.replacementTile = GridManager._instance.tilesPack.GetCircusTile;
            return copy;
        }

        public override void Init(Vector2Int position, TileMapLayer buildingLayer) {
            eventInstance = new ToothPasteEvent(this, Time.time + eventDelay, position);
            TimeManager._instance.AddEvent(eventInstance);
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class ObsidianTile : BlockTile
    {
        private ObsidianTile() { }
        public override void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
            Tilemap tilemap = GridManager._instance.GetTilemap(buildingLayer);
            tilemap.RemoveTileFlags((Vector3Int)gridPosition, TileFlags.LockColor);
            tilemap.SetColor((Vector3Int)gridPosition, new Color(0.9f, 0.9f, 1f, 0.7f));

        }
    }
    #endregion
}












