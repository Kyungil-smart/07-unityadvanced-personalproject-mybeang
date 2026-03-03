using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Mine : ObjectOnTile, IInteracterableMiner, IInitializable
{
    public ItemType itemType;
    [SerializeField] private GameObject _resourcePrefab;
    private bool IsLocked;
    [SerializeField] private GameObject _lockObject;
    private int _unlockCost;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void InitNumberOfConnectPoint()
    {
        PrintLog($"{gameObject.name} work init number of connect point.");
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.East, null));
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.South, null));
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.North, null));
        PrintLog($"{gameObject.name} makes {heads.Count} connect points.");
    }

    public override void PutOnTileHandler()
    {
        IsLocked = _lockObject != null;
        if (!IsLocked)
        {
            InitNumberOfConnectPoint();
        }
    }

    public override void TakeOffTileHandler() { }

    public void InteractMiner(Miner miner)
    {
        // ToDo. Object Pool 관리 필요
        GameObject item = Instantiate(_resourcePrefab);
        item.SetActive(false);
        miner.items.Enqueue(item.GetComponent<Item>());
    }

    public void Unlock()
    {
        if (PlayerStatusManager.Instance.Gold >= _unlockCost)
        {
            _lockObject?.GetComponent<Animator>()?.SetTrigger("Unlocked");
            InitNumberOfConnectPoint();
            IsLocked = false;
            _lockObject.SetActive(false);
        }
    }

    public Task InitDataAsync()
    {
        _unlockCost = DataManager.Instance.minerData[$"{gameObject.name}rSO"].unlockCost;
        if (_unlockCost > 0) IsLocked = true;
        
        return Task.CompletedTask;
    }
}
