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
    private Mine _mine;
    private GameObject _mineObj;
    private MinerData _minerData;
    public ConnectPoint head
    {
        get
        {
            if (heads.Count == 0) return null;
            return heads[0];
        }
        set
        {
            if (heads.Count == 0) heads.Add(value);
            else heads[0] = value;
        } 
    }

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
        foreach (var head in heads)
            ConnectToNeighbor(head);
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
        StopMining();
        
        PrintLog("주변 광산 탐색 시작");
        GameObject[] objects = GridManager.Instance.GetObjectsAroundTile(gridPos);
        if (objects.Length == 0) return false;
        PrintLog("주변 뭔가 있음");
        foreach (var obj in objects)
        {
            if (obj?.GetComponent<Mine>() != null)
            {
                PrintLog("주변 광산 있음");
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
            PrintLog($"{tail.direction} 은 연결 가능?");
            if (_mine.IsConnectWith(gameObject, tail))
            {
                tail.neighbor = _mineObj;
                PrintLog($"{tail.direction} 연결됨");
                StartMining();
                return;
            }
            PrintLog($"{tail.direction} 은 연결 안됨");
        }
        _mine = null;
    }

    private void StartMining()
    {
        PrintLog("일하기 시작");
        _animator.SetBool("IsPicking", true);
        _minerCoroutine = StartCoroutine(MiningRoutine());
    }

    private void StopMining()
    {
        PrintLog("퇴근");
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
    
    public Item InteractBeltGet()
    {
        if (items.Count == 0) return null;
        return items.Dequeue();
    }

    protected override void InitNumberOfConnectPoint()
    {
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.East, null));
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.South, null));
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.North, null));
        heads.Add(new ConnectPoint(ConnectPointType.Head, Direction.West, null));
        
        tails.Add(new ConnectPoint(ConnectPointType.Tail, Direction.East, null));
        tails.Add(new ConnectPoint(ConnectPointType.Tail, Direction.North, null));
        tails.Add(new ConnectPoint(ConnectPointType.Tail, Direction.South, null));
        tails.Add(new ConnectPoint(ConnectPointType.Tail, Direction.West, null));
    }
}
