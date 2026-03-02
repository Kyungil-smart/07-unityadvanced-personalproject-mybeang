using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    public bool isDead;
    public MonsterData data;

    public void UpdateStatus(MonsterData data)
    {
        this.data = data;
    }
}