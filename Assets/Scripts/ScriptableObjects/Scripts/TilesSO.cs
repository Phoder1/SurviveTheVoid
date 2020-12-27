using Assets.TilesData;
using UnityEngine;
[CreateAssetMenu(menuName = "Tiles Pack")]
public class TilesSO : ScriptableObject
{
    [SerializeField]
    private GenericTile moonTile;
    [SerializeField]
    private ToothPaste toothPasteTile;
    [SerializeField]
    private GenericTile circusTile;
    [SerializeField]
    private ObsidianTile obsidianTile;

    public GenericTile getMoonTile => moonTile.Clone();

    public ToothPaste getToothPasteTile => (ToothPaste)toothPasteTile.Clone();

    public GenericTile getCircusTile => circusTile.Clone();

    public ObsidianTile getObsidianTile => (ObsidianTile)obsidianTile.Clone();
}
