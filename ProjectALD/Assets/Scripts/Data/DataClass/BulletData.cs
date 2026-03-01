using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "GameData/BulletData")]
public class BulletData : ScriptableObject
{
    public int copperCnt;
    public int iconCnt;
    public int fireEleCnt;
    public int iceEleCnt;
    public float damage;
    public float attackRate;
    public DamageType[] damageTypes;
    public float range;
    public float speed;
    public float count;  // 객체 1개당 지닐 수 있는 탄환 개수
}
