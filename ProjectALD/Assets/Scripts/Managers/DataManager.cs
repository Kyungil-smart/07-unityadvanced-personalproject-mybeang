using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
    public Dictionary<string, BulletData> bulletData = new();
    public Dictionary<string, DamageMultiplierData> damageMultiplierData = new();
    public Dictionary<string, MinerData> minerData = new();
    public Dictionary<string, MonsterData> monsterData = new();
    public Dictionary<string, TowerData> towerData = new();
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        Init();
    }
    
    private void Init()
    {
        LoadAllData("bulletSO", bulletData);
        LoadAllData("minerSO", minerData);
        LoadAllData("dmSO", damageMultiplierData);
        LoadAllData("monsterSO", monsterData);
        LoadAllData("towerSO", towerData);
    }

    private void LoadAllData<T>(string labelName, Dictionary<string, T> dict) where T : ScriptableObject
    {
        var handle = Addressables.LoadAssetsAsync<T>(labelName);
        handle.Completed += (resultHandle) =>
        {
            IList<T> dataList = resultHandle.Result;
            foreach (var data in dataList)
            {
                dict.TryAdd(data.name, data);
                Debug.Log($"Load {data.name}'s data successful.");
            }
        };
    }

    
}
