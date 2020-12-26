using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
