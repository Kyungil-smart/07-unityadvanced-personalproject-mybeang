using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectPoolManager : MonoBehaviour, IInitializable
{
    public static ObjectPoolManager Instance;
    public Dictionary<string, GameObject> Prefabs;
    public Dictionary<string, Stack<GameObject>> Pool;
    [SerializeField] private List<GameObject> parents ;
    [SerializeField] private int poolSize = 20;
    
    public void Awake()
    {
        if (Instance == null) Instance = this;
        Prefabs = new();
        Pool = new();
    }

    private GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int objectId = DataManager.Instance.GetObjectId();
        GameObject gObject = Instantiate(prefab, position, rotation);
        GameObject parent = FindParent(prefab);
        if (parent == null) throw new NullReferenceException("Object pool prefab " + prefab.name + " not found");
        gObject.transform.parent = parent.transform;
        gObject.name = $"{gObject.name}-{objectId}";
        return gObject;
    }
        
    public void PushGameObject(GameObject go)
    {   // 반납
        if (go.transform.parent == null) throw new KeyNotFoundException($"{go.name}'s Parent not found");
        go.SetActive(false);
        Pool[go.transform.parent.name].Push(go);
    }
    
    public GameObject PopGameObject(string key)
    {   // 사용
        if (!Pool.ContainsKey(key))
            throw new KeyNotFoundException($"Key {key} not found");
        
        if (Pool[key].Count <= 0)
            for (int i = 0; i < poolSize; i++)
                PushGameObject(Create(Prefabs[key], Vector3.zero, Quaternion.identity));
        
        return Pool[key].Pop();
    }

    private GameObject FindParent(GameObject prefab)
    {
        foreach (GameObject parent in parents)
        {
            if (parent.name == prefab.name) return parent;
        }
        return null;
    }
    
    private void Init()
    {
        var handle = Addressables.LoadAssetsAsync<GameObject>("ObjectPoolPrefabs").WaitForCompletion();
        foreach (var prefab in handle)
        {
            Prefabs.Add(prefab.name, prefab);
            Pool.Add(prefab.name, new Stack<GameObject>());
            for (int i = 0; i < poolSize; i++)
                PushGameObject(Create(prefab, Vector3.zero, Quaternion.identity));
        }
    }
    
    public Task InitDataAsync()
    {
        Init();
        return Task.CompletedTask;
    }
}
