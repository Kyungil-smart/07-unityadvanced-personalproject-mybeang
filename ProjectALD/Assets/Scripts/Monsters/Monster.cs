using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    public MonsterData data;
    
    public MonsterData LoadMonsterData(string ScriptableObjectName)
        => DataManager.Instance.monsterData[ScriptableObjectName];
}
