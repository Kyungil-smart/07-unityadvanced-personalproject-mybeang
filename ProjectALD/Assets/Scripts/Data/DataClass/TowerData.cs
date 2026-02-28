using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "GameData/TowerData")]
public class TowerData : ScriptableObject
{
    public int incrementHP;
    public int maxUpgradeLevel;
    public int initUpgradeCost;
    public int incrementUpgradeCost;
    public float multiplierDamage;
    public float initDamageUpgradeCost;
    public float incrementDamageUpgradeCost;
    public float maxDamageUpgradeLevel;
}
