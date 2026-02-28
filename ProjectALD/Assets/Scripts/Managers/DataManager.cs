using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataManager : MonoBehaviour, IInitializable
{
    public static DataManager Instance { get; private set; }

    private int _objectId;
    public Dictionary<string, BulletData> bulletData = new();
    public Dictionary<string, DamageMultiplierData> damageMultiplierData = new();
    public Dictionary<string, MinerData> minerData = new();
    public Dictionary<string, MonsterData> monsterData = new();
    public Dictionary<string, TowerData> towerData = new();
    public UIMessageData uiMessageData;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private async Task LoadAllData<T>(string labelName, Dictionary<string, T> dict) where T : ScriptableObject
    {
        var handle = Addressables.LoadAssetsAsync<T>(labelName);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            IList<T> dataList = handle.Result;
            foreach (var data in dataList)
                dict.TryAdd(data.name, data);
        }
        
    }

    private Task LoadUIMessageData()
    {
        uiMessageData = new();
        return Task.CompletedTask;
    }

    public async Task InitDataAsync()
    {
        await LoadAllData("bulletSO", bulletData);
        await LoadAllData("minerSO", minerData);
        await LoadAllData("dmSO", damageMultiplierData);
        await LoadAllData("monsterSO", monsterData);
        await LoadAllData("towerSO", towerData);
        await LoadUIMessageData();
    }

    public int GetObjectId() => _objectId++;
    
}
