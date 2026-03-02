using System.Collections.Generic;
using UnityEngine;

public class SpawanManager : MonoBehaviour
{
    [SerializeField] List<GameObject> monsterPrefabs;
    private float minX = 50, maxX = 58, minY = -11, maxY = 11;
    private bool isSpawan = false;

    private void Update()
    {
        if (!GameManager.Instance.IsPause)
        {
            if (!isSpawan)
            {
                Spwan();
            }    
        }
    }
    
    private void Spwan()
    {
        // 몬스터 선정
        GameObject monsterPrefab = monsterPrefabs[0];
        // 몬스터 팝업
        GameObject monster = Instantiate(monsterPrefab, new Vector3(50, 0, 0), Quaternion.identity);
        // 몬스터 데이터 업데이트
        string soName = monsterPrefab.name;
        soName = $"{soName.Split('_')[0]}SO_{soName.Split('_')[1]}";
        MonsterData data = LoadMonsterData(soName);
        EnemyStatus status = monster.GetComponent<EnemyStatus>();
        status.UpdateStatus(data);
        
        // 출발 지시
        EnemyMovement movement = monster.GetComponent<EnemyMovement>();
        movement.CanMove = true;
        
        isSpawan = true;
    }
    
    public MonsterData LoadMonsterData(string ScriptableObjectName)
        => DataManager.Instance.monsterData[ScriptableObjectName];
}
