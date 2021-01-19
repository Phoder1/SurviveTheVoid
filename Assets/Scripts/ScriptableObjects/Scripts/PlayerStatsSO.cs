using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new PlayerStats", menuName = "SO/" + "PlayerStats")]
public class PlayerStatsSO : ScriptableObject
{
    [SerializeField] SurvivalStat[] survivalStats;
    [SerializeField] PlayerStat[] playerStats;
    [SerializeField] ExpStat expStat;

    public SurvivalStat[] GetSurvivalStats => survivalStats;
    public PlayerStat[] GetPlayerStats => playerStats;
    public ExpStat GetExpStat => expStat;
}
