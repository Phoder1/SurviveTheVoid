using Assets.TilesData;
using UnityEngine;
[CreateAssetMenu(menuName = "Tiles Pack")]
public class TilesPackSO : ScriptableObject
{
    [SerializeField] private BlockTile moonTile;
    [SerializeField] private ToothPaste toothPasteTile;
    [SerializeField] private BlockTile circusTile;
    [SerializeField] private ObsidianTile obsidianTile;
    [SerializeField] private GatherableTile TreeTile;
    public GatherableTile GetTree => (GatherableTile)TreeTile.Clone();
    public BlockTile GetMoonTile => (BlockTile)moonTile.Clone();
    public ToothPaste GetToothPasteTile => (ToothPaste)toothPasteTile.Clone();
    public BlockTile GetCircusTile => (BlockTile)circusTile.Clone();
    public ObsidianTile GetObsidianTile => (ObsidianTile)obsidianTile.Clone();
}
