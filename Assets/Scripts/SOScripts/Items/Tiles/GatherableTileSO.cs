using Assets.TimeEvents;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Gatherable Tile", menuName = "SO/" + "Tiles/" + "Gatehrable", order = 1)]
public class GatherableTileSO : TileAbstSO
{
    [SerializeField] private GrowthStage[] stages;
    [Min(1)]
    [SerializeField] private int sourceTier;
    [SerializeField] private int gatherDurabilityCost;
    [SerializeField] private Sound gatheringSound;
    [SerializeField] private ToolType toolType;
    [SerializeField] private float minGrowTime;
    [SerializeField] private float maxGrowTime;
    [SerializeField] private float gatheringTime;


    public GrowthStage[] GetStages => stages;
    public ToolType GetToolType => toolType;
    public Sound getGatheringSound => gatheringSound;
    public float GetMinGrowTime => minGrowTime;
    public float GetMaxGrowTime => maxGrowTime;
    public float GetGatheringTime => gatheringTime;
    public int GetSourceTier => sourceTier;
    public int GetGatherDurabilityCost => gatherDurabilityCost;
}
[System.Serializable]
public class GrowthStage
{
    [SerializeField] private float expReward;
    [SerializeField] private TileBase stageTile;
    [SerializeField] private TileBase tileWhileGathered;
    [SerializeField] private GameObject particlesWhileGathered;
    [SerializeField] private bool isGatherable, destroyOnGather, isSolid;
    [SerializeField] private Drop[] drops;

    public TileBase GetStageTile => stageTile;
    public GameObject GetParticlesWhileGathered => particlesWhileGathered;
    public TileBase GetTileWhileGathered => tileWhileGathered;
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
    private GameObject particlesObject;
    private readonly Vector3 particlesOffset = new Vector3(0, 2, 20);
    private readonly Quaternion particlesRotation = Quaternion.Euler(-60, 0, 0);
    public int currentStageIndex = 0;
    public int StagesCount => tile.GetStages.Length;
    private bool isBeingGathered;
    public bool reachedMaxStage => currentStageIndex >= StagesCount - 1;
    public void SetIsBeingGatheredState(bool state, Vector2Int gridPos) {
        if (isBeingGathered != state && currentStage.GetTileWhileGathered != null) {
            isBeingGathered = state;
            GridManager._instance.SetTile(tileSlot, gridPos, TileMapLayer.Buildings);
            if (currentStage.GetParticlesWhileGathered != null) {
                if (state) {
                Vector3 position = GridManager._instance.GridToWorldPosition(gridPos, TileMapLayer.Buildings, true) + particlesOffset;
                particlesObject = Object.Instantiate(currentStage.GetParticlesWhileGathered, position, particlesRotation);
                }else if(particlesObject != null){
                    Object.Destroy(particlesObject);
                    particlesObject = null;
                }
            }
        }
    }
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
                return (isBeingGathered ? currentStage.GetTileWhileGathered : currentStage.GetStageTile);
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
            expEffect.duration = currentStage.GetExpReward / expEffect.amount;
            effectController.Begin(expEffect);

            Debug.Log("Tried gathering");
            if (currentStage.GetDestroyOnGather || currentStageIndex == 0)
                Remove(gridPosition, tilemapLayer);
            else {
                currentStageIndex--;
                if (!reachedMaxStage)
                    InitEvent(gridPosition, tilemapLayer);
                SetIsBeingGatheredState(false, gridPosition);
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
        if (particlesObject != null) {
            Object.Destroy(particlesObject);
            particlesObject = null;
        }
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

