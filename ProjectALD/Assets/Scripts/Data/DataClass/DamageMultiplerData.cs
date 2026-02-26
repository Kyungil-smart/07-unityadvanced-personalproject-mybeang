using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageMuliplerData", menuName = "GameData/DamageMuliplerData")]
public class DamageMultiplierData : ScriptableObject
{
    public float toNormal;
    public float toStrongFire;
    public float toStrongIce;
    public float toStrongPhysic;
    public float toWeakFire;
    public float toWeakIce;
    public float toWeakPhysic;
}
