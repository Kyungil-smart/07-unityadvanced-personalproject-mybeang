using System.Collections;
using UnityEngine;

public class BridgeBelt : ObjectOnTile, IBelt, IInteractableBeltPut, IBeltBehavior, IMovableBuilding, IRotatable
{
    public Item item { get; set; }  // 초기 가로
    public Item upperItem; // 초기 세로
    private WaitForSeconds _deliveryInterval;
    private Coroutine _deliverItemRoutine;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _deliveryInterval = new WaitForSeconds(1f); // ToDO. 게임벨런스 패치
        InitNumberOfConnectPoint();
    }

    private void Update()
    {
        if (IsConnected() && _deliverItemRoutine == null)
        {
            StartOperation();
        }
        else if (!IsConnected() && _deliverItemRoutine != null)
            StopOperation();
    }
    
    protected override void InitNumberOfConnectPoint()
    {
        // 초기 기준 0번이 가로, 1번이 세로.
        tails.Add(new(ConnectPointType.Tail, Direction.West, null));
        tails.Add(new(ConnectPointType.Tail, Direction.South, null));
        heads.Add(new(ConnectPointType.Head, Direction.East, null));
        heads.Add(new(ConnectPointType.Head, Direction.North, null));
    }

    public override void PutOnTileHandler()
    {
        _spriteRenderer.sortingLayerName = "Belt";
        foreach(var head in heads)
            ConnectToNeighbor(head);
        foreach(var tail in tails)
            ConnectToNeighbor(tail);
    }

    public override void TakeOffTileHandler()
    {
        foreach(var head in heads)
            head.neighbor = null;
        foreach(var tail in tails)
            tail.neighbor = null;
    }

    private void StartOperation()
    {
        _deliverItemRoutine = StartCoroutine(DeliverItemCoroutine());
    }

    private void StopOperation()
    {
        if (_deliverItemRoutine != null) StopCoroutine(_deliverItemRoutine);
        _deliverItemRoutine = null;
    }

    public IEnumerator DeliverItemCoroutine()
    {
        while (true)
        {
            GetItem();
            PutItem();
            yield return _deliveryInterval;
        }
    }
    
    public void GetItem()
    {
        if (item == null) item = tails[0].getFromNeighbor?.InteractBeltGet();
        if (upperItem == null) upperItem = tails[1].getFromNeighbor?.InteractBeltGet();
    }

    public void PutItem()
    {
        if (item != null)
        {
            heads[0].putToNeighbor?.InteractBeltPut(item);
            item = null;
        }
        if (upperItem != null)
        {
            heads[1].putToNeighbor?.InteractBeltPut(upperItem);
            upperItem = null;
        }
    }

    private bool IsConnected()
    {
        if (heads.Count == 0 || tails.Count == 0) return false;
        
        if (heads[0].neighbor == null && tails[0].neighbor == null) return false;
        if (heads[1].neighbor == null && tails[1].neighbor == null) return false;
        return true;
    }

    public void Rotate()
    {
        // 동작 멈춤
        StopOperation();
        // 아이템 비움
        item = null;
        // 이미지 변경
        RotateImage();
        
        // Connect Point 변경
        foreach(var head in heads)
            head.RotateConnectPoint();
        foreach(var tail in tails)
            tail.RotateConnectPoint();
        if (myTile != null)
        {
            // 타일 위에 이미 지어진 것이면 다시 Connection 시도.
            foreach(var head in heads)
                ConnectToNeighbor(head);
            foreach(var tail in tails)
                ConnectToNeighbor(tail);    
        }
    }

    private void RotateImage()
    {
        _animator?.SetTrigger("Rotate");
    }
    
    public void InteractBeltPut(Item acquiredItem)  // 어디를 통해 들어온 녀석일까?
    {
        // 이걸 어떻게 처리한담?
    }
}
