using UnityEngine;
[CreateAssetMenu(menuName = "Tiles Pack")]
public class TilesSO : ScriptableObject
{
    public NoiseSO floorVariationNoise;
    public NoiseSO buildingsVariationNoise;
    public MoonTile moonTile;
    public ToothPaste toothPasteTile;
    public CircusTile circusTile;
    public ObsidianTile obsidianTile;
}
