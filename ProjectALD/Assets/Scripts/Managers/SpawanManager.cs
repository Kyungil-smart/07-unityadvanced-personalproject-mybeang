using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawanManager : MonoBehaviour
{
    public static SpawanManager Instance;  // 흠..
    [SerializeField] List<GameObject> monsterPrefabs;
    private float minX = 50, maxX = 58, minY = -5, maxY = 5;
    private WaitForSeconds _spwanInterval;
    private Coroutine _spwanCoroutine;
    private Dictionary<string, (int acc, int max)> _spawnCount;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        _spwanInterval = new WaitForSeconds(30f);  // 1분으로 늘리기
        _spawnCount = new();
    }
    
    private void Update()
    {
        if (GameManager.Instance.gameState == GameState.WaveStart && _spwanCoroutine == null)
        {
            Debug.Log("Start Spwan Enemies !!!!");
            _spwanCoroutine = StartCoroutine(SpwanCoroutine());
        }

        if (_spwanCoroutine != null &&
            (GameManager.Instance.gameState == GameState.GameWon
             || GameManager.Instance.gameState == GameState.GameOver))
        {
            StopCoroutine(_spwanCoroutine);
            _spwanCoroutine = null;
        }
    }

    private IEnumerator SpwanCoroutine()
    {
        while (true)
        {
            Spwan();
            yield return _spwanInterval;    
        }
    }
    
    private void Spwan()
    {
        // 몬스터 선정
        foreach (GameObject monsterPrefab in monsterPrefabs)
        {
            // 몬스터 데이터 확보
            string soName = monsterPrefab.name;
            soName = $"{soName.Split('_')[0]}SO_{soName.Split('_')[1]}";
            MonsterData data = LoadMonsterData(soName);
            if (!_spawnCount.ContainsKey(soName))
            {
                _spawnCount.Add(soName, (0, data.maxAvailableSpawnCount));
            }
            Debug.Log($"Spawner: {soName} {data.firstSpawnWave} {GameManager.Instance.CurrentWave}");
            if (data.firstSpawnWave > GameManager.Instance.CurrentWave)
            {
                Debug.Log($"Spawner: {soName} not spawn, yet.");
                continue;
            }
            // Wave 별 몬스터 Spwan 계산
            int curWaveSpawnCnt = data.firstSpawnCount + data.incrementCountForWave * (GameManager.Instance.CurrentWave - 1);
            for (int i = 0; i < curWaveSpawnCnt; i++)
            {
                if (_spawnCount[soName].acc <= _spawnCount[soName].max)
                {
                    Vector3 spawnPos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
                    // 몬스터 팝업
                    GameObject monster = ObjectPoolManager.Instance.PopGameObject(monsterPrefab.name);
                    monster.transform.position = spawnPos;
                    monster.SetActive(true);
                
                    // 몬스터 HP 조절
                    float hp = data.HP;
                    hp += hp * (curWaveSpawnCnt - 1) * data.multiplierHpForWave;
                
                    // 몬스터 데이터 업데이트
                    EnemyStatus status = monster.GetComponent<EnemyStatus>();
                    status.UpdateStatus(data, hp, soName);
                
                    // 출발
                    EnemyMovement movement = monster.GetComponent<EnemyMovement>();
                    movement.CanMove = true;
                    
                    _spawnCount[soName] = (_spawnCount[soName].acc + 1, _spawnCount[soName].max);
                }
            }
        }
    }

    public void DecreseSpawnCount(string soName)
    {
        if (_spawnCount.ContainsKey(soName))
            _spawnCount[soName] = (_spawnCount[soName].acc - 1, _spawnCount[soName].max);
    }
    
    public MonsterData LoadMonsterData(string ScriptableObjectName)
        => DataManager.Instance.monsterData[ScriptableObjectName];
}
