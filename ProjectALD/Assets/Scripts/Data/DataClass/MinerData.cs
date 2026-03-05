using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinerData", menuName = "GameData/MinerData")]
public class MinerData : ScriptableObject
{
    public int initMineCount;
    public int incrementMineCount;
    public int maxMineUpgradeLevel;
    public int unlockCost;
    public int initUpgradeCost;
    public int incrementUpgradeCost;
}
