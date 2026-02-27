using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Mine : ObjectOnTile, IInteracterableMiner, IInitializable
{
    public ResourceType resourceType;
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
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.East, null));
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.South, null));
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.North, null));
    }

    public override void PutOnTileHandler()
    {
        IsLocked = _lockObject != null;
        if (!IsLocked)
        {
            InitNumberOfConnectPoint();
        }
    }

    public void InteractMiner(Miner miner)
    {
        GameObject item = Instantiate(_resourcePrefab);
        item.SetActive(false);
        miner.items.Enqueue(item);
    }

    public void Unlock()
    {
        if (GameManager.Instance.Gold >= _unlockCost)
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
