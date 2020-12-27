﻿using Assets.TimeEvents;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#region Notes!
/*
* How to create a new Tile:
* 1) Create a new child class to TileAbst
* 2) Fill the abstract import variables
* 3) Add whatever you want from there
*/
#endregion
public class Tiles : MonoBehaviour
{
    public TilesSO tiles;
    public static Tiles _instance;
    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
    }
}
#region General tile abstract class and enum
public enum ToolInteraction { None, Axe, Pickaxe, Hoe, Shovel, Hammer, Any }
[Serializable]
public abstract class TileAbst
{
    public TileBase mainTileBase;
    public ToolInteraction interactionType;
    public bool isActiveInteraction;
    protected TimeEvent eventInstance;
    public bool isSolid;
    public virtual void Init(Vector2Int _position, BuildingLayer buildingLayer) { }
    public virtual void Remove() {
        if (eventInstance != null) {
            eventInstance.Cancel();
        }
    }
    public abstract void ImportVariables(Vector2Int gridPosition, BuildingLayer buildingLayer);
    public virtual void GatherInteraction(Vector2Int gridPosition, BuildingLayer buildingLayer) { }
    public virtual void LongPressUpdate() { }

    [Serializable]
    public struct TileVariations
    {
        public TileBase tileBase;
        public float chanceWeight;
    }

}
public struct TileHit
{
    public readonly Vector2Int gridPosition;
    public readonly TileAbst tile;

    public TileHit(TileAbst tile,   Vector2Int gridPosition) {
        this.tile = tile;
        this.gridPosition = gridPosition;
    }

    public static TileHit None => new TileHit(null, Vector2Int.zero);

    public static bool operator ==(TileHit lhs, TileHit rhs) {
        return lhs.Equals(rhs);
    }
    public static bool operator !=(TileHit lhs, TileHit rhs) {
        return !lhs.Equals(rhs);
    }

    public override bool Equals(object obj) {
        return (obj is TileHit hit &&
               EqualityComparer<Vector2Int?>.Default.Equals(gridPosition, hit.gridPosition) &&
               EqualityComparer<TileAbst>.Default.Equals(tile, hit.tile));
    }
    public bool Equals(TileHit other) {
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
#region Actual Tiles
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
[Serializable]
public class MoonTile : TileAbst
{

    public override void ImportVariables(Vector2Int gridPosition, BuildingLayer buildingLayer) {
        MoonTile SO = Tiles._instance.tiles.moonTile;
        mainTileBase = SO.mainTileBase;
        interactionType = SO.interactionType;
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
[Serializable]
public class CircusTile : TileAbst
{
    public override void ImportVariables(Vector2Int gridPosition, BuildingLayer buildingLayer) {
        CircusTile SO = Tiles._instance.tiles.circusTile;
        mainTileBase = SO.mainTileBase;
        interactionType = SO.interactionType;
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
[Serializable]
public class ToothPaste : TileAbst
{
    public TileAbst replacementTile;
    public int eventDelay;

    public override void ImportVariables(Vector2Int gridPosition, BuildingLayer buildingLayer) {
        ToothPaste SO = Tiles._instance.tiles.toothPasteTile;
        mainTileBase = SO.mainTileBase;
        eventDelay = SO.eventDelay;
        interactionType = SO.interactionType;
        replacementTile = new CircusTile();
    }

    public override void Init(Vector2Int position, BuildingLayer buildingLayer) {
        eventInstance = new ToothPasteEvent(this, Time.time + eventDelay, position);
        TimeManager._instance.AddEvent(eventInstance);
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
[Serializable]
public class ObsidianTile : TileAbst
{
    public override void GatherInteraction(Vector2Int gridPosition, BuildingLayer buildingLayer) {
        Tilemap tilemap = GridManager._instance.GetTilemap(buildingLayer);
        tilemap.RemoveTileFlags((Vector3Int)gridPosition, TileFlags.LockColor);
        tilemap.SetColor((Vector3Int)gridPosition, new Color(0.9f, 0.9f, 1f, 0.7f));

    }

    public override void ImportVariables(Vector2Int gridPosition, BuildingLayer buildingLayer) {
        ObsidianTile SO = Tiles._instance.tiles.obsidianTile;
        mainTileBase = SO.mainTileBase;
        interactionType = SO.interactionType;
    }
}
#endregion












