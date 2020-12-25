using Assets.TimeEvents;
using System;
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
    public static Tiles _instance;
    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
        GenerateSeeds();
    }

    private void GenerateSeeds() {
        tiles.floorVariationNoise.GenerateSeed();
        tiles.buildingsVariationNoise.GenerateSeed();
    }

    #region Tiles import assets scriptable object
    public TilesSO tiles;
    [CreateAssetMenu(menuName = "Tiles Pack")]
    public class TilesSO : ScriptableObject
    {
        public Noise floorVariationNoise;
        public Noise buildingsVariationNoise;
        public MoonTile moonTile;
        public ToothPaste toothPasteTile;
        public CircusTile circusTile;
        public ObsidianTile obsidianTile;
    }
    #endregion
    #region Actual Tiles
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class MoonTile : TileAbst
    {

        public override void ImportVariables(Vector2Int gridPosition, BuildingLayer buildingLayer) {
            MoonTile SO = _instance.tiles.moonTile;
            mainTileBase = SO.PickTileBase(gridPosition, buildingLayer);
            interactionType = SO.interactionType;
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class CircusTile : TileAbst
    {
        public override void ImportVariables(Vector2Int gridPosition, BuildingLayer buildingLayer) {
            CircusTile SO = _instance.tiles.circusTile;
            mainTileBase = SO.PickTileBase(gridPosition, buildingLayer);
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
            ToothPaste SO = _instance.tiles.toothPasteTile;
            mainTileBase = SO.PickTileBase(gridPosition, buildingLayer);
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
            tilemap.SetColor((Vector3Int)gridPosition, new Color(0.9f,0.9f,1f,0.7f));

        }

        public override void ImportVariables(Vector2Int gridPosition, BuildingLayer buildingLayer) {
            ObsidianTile SO = _instance.tiles.obsidianTile;
            mainTileBase = SO.PickTileBase(gridPosition, buildingLayer);
            interactionType = SO.interactionType;
        }
    }
    #endregion
}
#region General tile abstract class and enum
public enum ToolInteraction { None, Axe, Pickaxe, Hoe, Shovel, Hammer, Any }
[Serializable]
public abstract class TileAbst
{
    [HideInInspector]
    public TileBase mainTileBase;
    public TileVariations[] tileBaseVariations;
    public ToolInteraction interactionType;
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
    public virtual TileBase PickTileBase(Vector2Int gridPosition, Noise noise) {
        //Check for variations
        if (tileBaseVariations.Length == 1) {
            return tileBaseVariations[0].tileBase;
        }
        //Check if tile was set
        if (tileBaseVariations.Length == 0) {
            Debug.LogWarning("Tile not set!");
            return null;
        }

        float weightSum = 0;
        foreach (TileVariations tile in tileBaseVariations) {
            weightSum += tile.chanceWeight;
        }

        float roll = Mathf.Clamp(noise.GetRandomValue(gridPosition), 0f, 1f);

        roll *= weightSum;
        foreach (TileVariations tile in tileBaseVariations) {
            roll -= tile.chanceWeight;
            if (roll <= 0) {
                return tile.tileBase;
            }
        }
        Debug.LogError("Roll out of bounds!");
        return tileBaseVariations[0].tileBase;

    }
    public virtual TileBase PickTileBase(Vector2Int gridPosition, BuildingLayer buildingLayer) {
        switch (buildingLayer) {
            case BuildingLayer.Floor:
                return PickTileBase(gridPosition, Tiles._instance.tiles.floorVariationNoise);
            case BuildingLayer.Buildings:
                return PickTileBase(gridPosition, Tiles._instance.tiles.buildingsVariationNoise);
            default:
                throw new NotImplementedException();
        }
    }

    [Serializable]
    public struct TileVariations
    {
        public TileBase tileBase;
        public float chanceWeight;
    }

}

#endregion













