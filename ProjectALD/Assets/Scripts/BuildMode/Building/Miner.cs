using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : ObjectOnTile, IInteractableBeltGet
{
    [SerializeField] private int _currentResourceCount;
    [SerializeField] private int _maxResourceCount;

    public Queue<GameObject> item;
    private WaitForSeconds _oneSecond = new WaitForSeconds(1f);
    private Coroutine _minerCoroutine;
    private bool _connectedMine;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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

    public override void PutOnTileHandler()
    {
        SearchMine();
    }
    
    private void SearchMine()
    {
        // Stop Mining before search.
        _connectedMine = false;
        StopMining();
        
        GameObject[] objects = GridManager.Instance.GetObjectsAroundTile(myTile.GridPos);
        if (objects.Length == 0 || objects.Length > 1) return;
        GameObject mine = objects[0];
        
        if (mine.GetComponent<IInteracterableMiner>() != null)
            ConnectToMine(mine);
    }

    private void ConnectToMine(GameObject mineObj)
    {
        Mine mine = mineObj.GetComponent<IInteracterableMiner>() as Mine;
        foreach (ConnectPoint tail in tails)
        {
            if (mine.IsConnectWith(gameObject, tail))
            {
                tail.neighbor = mineObj;
                _connectedMine = true;
                StartMining();
                return;
            }
        }
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
        while (_currentResourceCount <= _maxResourceCount)
        {
            _currentResourceCount++;
            yield return _oneSecond;
        }
    }
    
    public void InteractBeltGet(BaseBelt baseBelt)
    {
        if (_currentResourceCount > 0)
            _currentResourceCount -= 1;
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
