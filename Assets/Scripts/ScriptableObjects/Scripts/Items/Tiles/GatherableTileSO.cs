﻿using Assets.TimeEvents;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Gatherable Tile", menuName = "SO/" + "Tiles/" + "Gatehrable", order = 1)]
public class GatherableTileSO : TileAbstSO
{
    [SerializeField] private GrowthStage[] stages;
    [Min(1)]
    [SerializeField] private int sourceTier;
    [SerializeField] private Sound gatheringSound;
    public GrowthStage[] GetStages => stages;
    [SerializeField] private ToolType toolType;


    [SerializeField] private float minGrowTime;
    [SerializeField] private float maxGrowTime;
    [SerializeField] private float GatheringTime;
    public ToolType GetToolType => toolType;
    public Sound getGatheringSound => gatheringSound;
    public float GetMinGrowTime => minGrowTime;
    public float GetMaxGrowTime => maxGrowTime;
    public float GetGatheringTime => GatheringTime;
}
[System.Serializable]
public class GrowthStage
{
    [SerializeField] private float expReward;
    [SerializeField] private TileBase stageTile;
    [SerializeField] private bool isGatherable, destroyOnGather, isSolid;
    [SerializeField] private Drop[] drops;

    public TileBase GetStageTile => stageTile;
    public bool GetIsGatherable => isGatherable;
    public bool GetIsSolid => isSolid;
    public bool GetDestroyOnGather => destroyOnGather;
    public float GetExpReward => expReward;
    public Drop[] GetDrops => drops;
}
[System.Serializable]
public class Drop
{
    [SerializeField] ItemSO item;
    [SerializeField] float Chance;
    [SerializeField] int minAmount;
    [SerializeField] int maxAmount;

    public ItemSO GetItem => item;
    public float GetChance => Chance;
    public int GetMinAmount => minAmount;
    public int GetMaxAmount => maxAmount;
}
public class GatherableState : ITileState
{
    public TileSlot tileSlot;
    public TimeEvent eventInstance;
    public GatherableTileSO tile;
    public int currentStageIndex = 0;
    public int StagesCount => tile.GetStages.Length;
    public bool reachedMaxStage => currentStageIndex >= StagesCount - 1;
    private GrowthStage currentStage => tile.GetStages[currentStageIndex];
    private static EffectData expEffect = new EffectData(StatType.EXP, EffectType.OverTime, 10f, 1, 0.03f);

    public GatherableState(GatherableTileSO tile, TileSlot tileSlot) {
        currentStageIndex = 0;
        this.tile = tile;
        this.tileSlot = tileSlot;
    }
    public TileBase GetMainTileBase {
        get {
            if (tile.GetStages != null)
                return currentStage.GetStageTile;
            return tile.GetMainTileBase;
        }

    }
    public TileAbstSO GetTileAbst => tile;

    public ToolType GetToolType => tile.GetToolType;

    public TileType GetTileType => tile.GetTileType;
    public Sound getGatheringSound => tile.getGatheringSound;

    public bool GetIsSolid {
        get {
            if (StagesCount > 0) {
                return currentStage.GetIsSolid;
            }
            else {
                return tile.GetIsSolid;
            }

        }
    }
    public float GetGatherTime => GetGatherTime;

    public bool isSpecialInteraction => tile.isSpecialInteraction;
    public bool GetIsGatherable => currentStage.GetIsGatherable;

    public void GatherInteraction(Vector2Int gridPosition, TileMapLayer tilemapLayer) {
        if (GetIsGatherable) {
            EffectController effectController = new EffectController(PlayerStats._instance.GetStat(StatType.EXP), 0);
            expEffect.duration = currentStage.GetExpReward/expEffect.amount;
            effectController.Begin(expEffect);

            Debug.Log("Tried gathering");
            if (currentStage.GetDestroyOnGather || currentStageIndex == 0)
                Remove(gridPosition, tilemapLayer);
            else {
                currentStageIndex--;
                if (!reachedMaxStage)
                    InitEvent(gridPosition, tilemapLayer);
                GridManager._instance.SetTile(tileSlot, gridPosition, tilemapLayer, true);
            }

            Inventory inventory = Inventory.GetInstance;
            foreach (Drop drop in currentStage.GetDrops) {
                if (Random.value <= drop.GetChance) {
                    inventory.AddToInventory(0, new ItemSlot(drop.GetItem, Random.Range(drop.GetMinAmount, drop.GetMaxAmount + 1)));
                }
            }


        }
    }
    private void Remove(Vector2Int gridPosition, TileMapLayer tilemapLayer) {
        CancelEvent(gridPosition, tilemapLayer);
        GridManager._instance.SetTile(null, gridPosition, tilemapLayer, true);
    }
    public void CancelEvent(Vector2Int gridPosition, TileMapLayer tilemapLayer) {
        if (eventInstance != null)
            eventInstance.Cancel();
    }

    public void SpecialInteraction(Vector2Int gridPosition, TileMapLayer tileMapLayer) {
    }
    public void Grow(Vector2Int gridPosition, TileMapLayer tileMapLayer) {
        currentStageIndex++;
        GridManager._instance.SetTile(tileSlot, gridPosition, tileMapLayer, false);
        if (!reachedMaxStage) {
            InitEvent(gridPosition, tileMapLayer);
        }
    }
    public void Init(Vector2Int gridPosition, TileMapLayer tilemapLayer, bool generation = false) {
        if (generation) {
            currentStageIndex = StagesCount - 1;
        }
        if (eventInstance == null && tile.GetStages.Length > 1 && !reachedMaxStage)
            InitEvent(gridPosition, tilemapLayer);
    }
    private void InitEvent(Vector2Int gridPosition, TileMapLayer tilemapLayer) {
        if (eventInstance != null)
            CancelEvent(gridPosition, tilemapLayer);
        eventInstance = new TileGrowEvent(Time.time + Random.Range(tile.GetMinGrowTime, tile.GetMaxGrowTime), tileSlot, gridPosition, tilemapLayer);
    }


}
public class TileGrowEvent : TimeEvent
{
    protected TileSlot triggeringTile;
    protected readonly Vector2Int eventPosition;
    protected readonly TileMapLayer tileMapLayer;
    public TileGrowEvent(float triggerTime, TileSlot triggeringTile, Vector2Int eventPosition, TileMapLayer tileMapLayer) : base(triggerTime) {
        this.triggeringTile = triggeringTile;
        this.eventPosition = eventPosition;
        this.tileMapLayer = tileMapLayer;
    }

    protected override void TriggerBehaviour() {
        ((GatherableState)triggeringTile.tileState).Grow(eventPosition, tileMapLayer);

    }
}

