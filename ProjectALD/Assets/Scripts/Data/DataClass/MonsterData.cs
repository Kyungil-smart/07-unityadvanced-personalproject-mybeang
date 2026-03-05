using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "GameData/MonsterData")]
public class MonsterData : ScriptableObject
{
    public int HP;
    public int damage;
    public int moveSpeed;
    public List<MonsterAmmorType> amorType;
    public MonsterAttackType attackType;
    public float attackRange;
    public int maxAvailableSpawnCount;
    public int firstSpawnCount;
    public float multiplierHpForWave;
    public int firstSpawnWave;
    public int incrementCountForWave;
}
