using System;
using System.Collections;
using UnityEngine;

public class Miner : ObjectOnTile, IInteractableBeltGet
{
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private int _currentResourceCount;
    [SerializeField] private int _maxResourceCount;

    private Item _item;
    private WaitForSeconds oneSecond = new WaitForSeconds(1f);
    private Coroutine _minerCoroutine;
    private GameObject _mine;

    private void OnEnable()
    {
        _minerCoroutine = null;
    }

    private void OnDisable()
    {
        if(_minerCoroutine != null) StopCoroutine(_minerCoroutine);
    }

    private GameObject CheckMine()
    {
        GameObject[] objects = GridManager.Instance.GetObjectsAroundTile(myTile.GridPos);
        if (objects.Length == 0 || objects.Length > 1) return null;
        return objects[0];
    }

    private void ConnectToMine(GameObject mine)
    {
        _mine = mine;
    }

    private void StartMining()
    {
        _minerCoroutine = StartCoroutine(MiningRoutine());
    }

    private IEnumerator MiningRoutine()
    {
        while (_currentResourceCount <= _maxResourceCount)
        {
            if (_item)
                _currentResourceCount++;
            yield return oneSecond;
        }
    }
    
    public void InteractBeltGet(BaseBelt baseBelt)
    {
        if (_currentResourceCount > 0)
        {
            _currentResourceCount -= 1;
        }

        baseBelt.item = _item;
    }
}
