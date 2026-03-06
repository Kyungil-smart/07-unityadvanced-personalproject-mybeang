using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Mine : ObjectOnTile, IInteracterableMiner, IInitializable
{
    public ItemType itemType;
    [SerializeField] private GameObject _resourcePrefab;
    public bool isLocked;
    [SerializeField] private GameObject _lockObject;
    private int _unlockCost;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void InitNumberOfConnectPoint()
    {
        PrintLog($"{gameObject.name} work init number of connect point.");
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.East, null, gameObject));
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.South, null, gameObject));
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.North, null, gameObject));
        PrintLog($"{gameObject.name} makes {heads.Count} connect points.");
    }

    public override void PutOnTileHandler()
    {
        isLocked = _lockObject != null;
        if (!isLocked)
        {
            InitNumberOfConnectPoint();
        }
    }

    public override void TakeOffTileHandler() { }

    public void InteractMiner(Miner miner)
    {
        GameObject item = ObjectPoolManager.Instance.PopGameObject(_resourcePrefab.name);
        item.SetActive(false);
        miner.items.Enqueue(item.GetComponent<Item>());
    }

    public void Unlock()
    {
        if (PlayerStatusManager.Instance.Gold >= _unlockCost)
        {
            _lockObject?.GetComponent<Animator>()?.SetTrigger("Unlocked");
            InitNumberOfConnectPoint();
            isLocked = false;
            _lockObject?.SetActive(false);
            PlayerStatusManager.Instance.WastGold(_unlockCost);
        }
    }

    public Task InitDataAsync()
    {
        _unlockCost = DataManager.Instance.minerData[$"{gameObject.name}rSO"].unlockCost;
        if (_unlockCost > 0) isLocked = true;
        return Task.CompletedTask;
    }
}
