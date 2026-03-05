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
    public Dictionary<string, int> buildCostData = new();  // 추후 외부에서 데이터를 어떻게 받아오면 좋을지 고민..
    public UIMessageData uiMessageData;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // Building 별 가격 책정. 추후 Inspector 나 외부 데이터화 해서 가져오는 것을 염두해야 한다.
        // 현재는 너무 간단한 데이터며, 다른 데이터가 이미 SO 로 정의되어 있고, 프로젝트 기간상 연구시간이 부족해 아래와 같이 정의한다.
        buildCostData["Miner"] = 2000;
        buildCostData["Factory"] = 1000;
        buildCostData["Tower"] = 500;
    }

    private async Task LoadAllData<T>(string labelName, Dictionary<string, T> dict) where T : ScriptableObject
    {
        var handle = Addressables.LoadAssetsAsync<T>(labelName);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            IList<T> dataList = handle.Result;
            foreach (var data in dataList)
            {
                dict.TryAdd(data.name, data);
            }
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
