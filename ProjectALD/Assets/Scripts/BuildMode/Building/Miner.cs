using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ToDo: 개별 UI 개발 후 Upgrade Logic 구현 

public class Miner : ObjectOnTile, IInteractableBeltGet, IMovableBuilding
{
    [SerializeField] private int _currentResourceCount;
    [SerializeField] private int _maxResourceCount;

    public Queue<Item> items = new();
    private WaitForSeconds _oneSecond = new WaitForSeconds(1f);
    private Coroutine _minerCoroutine;
    private bool _connectedMine;
    private Mine _mine;
    private GameObject _mineObj;
    
    private MinerData _minerData;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        InitNumberOfConnectPoint();
    }
    
    private void OnEnable()
    {
        _minerCoroutine = null;
    }

    private void OnDisable()
    {
        StopMining();
    }

    public void DataLoad()
    {
        if (_mine == null) return;
        string key = $"{_mine.itemType}MinerSO";
        _minerData = DataManager.Instance.minerData[key];
    }

    public override void PutOnTileHandler()
    {
        _spriteRenderer.sortingLayerName = "FactoryMine";
        if (SearchMine(myTile.GridPos))
        {
            ConnectToMine();
            DataLoad();    
        }
    }

    public override void TakeOffTileHandler()
    {
        // ToDo: Move 시 추가 구현 필요.
    }

    public bool SearchMine(Vector2Int gridPos)
    {
        // Stop Mining before search.
        _connectedMine = false;
        StopMining();
        
        GameObject[] objects = GridManager.Instance.GetObjectsAroundTile(gridPos);
        if (objects.Length == 0) return false;
        foreach (var obj in objects)
        {
            if (obj?.GetComponent<Mine>() != null)
            {
                _mineObj = obj;
                _mine = obj.GetComponent<IInteracterableMiner>() as Mine;
                return true;
            }
        }
        return false;
    }

    private void ConnectToMine()
    {
        foreach (ConnectPoint tail in tails)
        {
            if (_mine.IsConnectWith(gameObject, tail))
            {
                tail.neighbor = _mineObj;
                _connectedMine = true;
                StartMining();
                return;
            }
        }
        _mine = null;
    }

    private void StartMining()
    {
        _animator.SetBool("IsPicking", true);
        _minerCoroutine = StartCoroutine(MiningRoutine());
    }

    private void StopMining()
    {
        _animator.SetBool("IsPicking", false);
        if(_minerCoroutine != null) StopCoroutine(_minerCoroutine);
        _minerCoroutine = null;
    }

    private IEnumerator MiningRoutine()
    {
        while (true)
        {
            if (_currentResourceCount < _maxResourceCount)
            {
                if (!_animator.GetBool("IsPicking")) 
                    _animator.SetBool("IsPicking", true);
                
                if (_mine != null)
                {
                    _mine.InteractMiner(this);
                    _currentResourceCount = items.Count;    
                }    
            }
            else
            {
                _animator.SetBool("IsPicking", false);
            }
            yield return _oneSecond;
        }
    }
    
    public void InteractBeltGet<T>(T belt) where T : ObjectOnTile, IBelt
    {
        if (items.Count == 0) return;
        if (belt.item == null)
            belt.item = items.Dequeue();
    }

    protected override void InitNumberOfConnectPoint()
    {
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.East, null));
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.South, null));
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.West, null));
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.North, null));
        
        tails.Add(new ConnectPoint(ConnectPointType.Tail, Direction.East, null));
        tails.Add(new ConnectPoint(ConnectPointType.Tail, Direction.South, null));
        tails.Add(new ConnectPoint(ConnectPointType.Tail, Direction.West, null));
        tails.Add(new ConnectPoint(ConnectPointType.Tail, Direction.North, null));
    }
}
