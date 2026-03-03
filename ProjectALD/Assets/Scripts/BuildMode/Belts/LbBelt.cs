using System.Collections;
using UnityEngine;

public class LbBelt : ObjectOnTile, IBelt, IInteractableBeltPut, IBeltBehavior, IMovableBuilding, IRotatable
{
    public Item item { get; set; }
    private WaitForSeconds _deliveryInterval;
    private Coroutine _deliverItemRoutine;
    private int rrIndex;

    public ConnectPoint tail
    {
        get
        {
            if (tails.Count == 0) return null;
            return tails[0];
        }
        set
        {
            if (tails.Count == 0) tails.Add(value);
            else tails[0] = value;
        }
    }
    
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
        tails.Add(new(ConnectPointType.Tail, Direction.South, null));
        heads.Add(new(ConnectPointType.Head, Direction.West, null));
        heads.Add(new(ConnectPointType.Head, Direction.North, null));
        heads.Add(new(ConnectPointType.Head, Direction.East, null));
    }

    public override void PutOnTileHandler()
    {
        _spriteRenderer.sortingLayerName = "Belt";
        foreach(var head in heads)
            ConnectToNeighbor(head);
        ConnectToNeighbor(tail);
    }

    public override void TakeOffTileHandler()
    {
        foreach(var head in heads)
            head.neighbor = null;
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

    private int RoundRobin(int index) => (++index) % 3;
    
    public void GetItem()
    {
        if (item == null) item = tail.getFromNeighbor?.InteractBeltGet();
    }

    public void PutItem()
    {
        if (item != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (heads[rrIndex].putToNeighbor != null)
                {
                    heads[rrIndex].putToNeighbor.InteractBeltPut(item);
                    item = null;
                    rrIndex = RoundRobin(rrIndex);
                    break;
                }
                rrIndex = RoundRobin(rrIndex);    
            }    
        }
    }

    private bool IsConnected()
    {
        if (heads.Count == 0 || tails.Count == 0) return false;
        
        if (tails[0].neighbor == null && 
            (heads[0].neighbor == null || heads[1].neighbor == null || heads[2].neighbor == null)) 
            return false;
        
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
        tail.RotateConnectPoint();
        if (myTile != null)
        {
            // 타일 위에 이미 지어진 것이면 다시 Connection 시도.
            foreach(var head in heads)
                ConnectToNeighbor(head);
            ConnectToNeighbor(tail);    
        }
    }

    private void RotateImage()
    {
        _animator?.SetTrigger("Rotate");
    }
    
    public void InteractBeltPut(Item acquiredItem)  // 어디를 통해 들어온 녀석일까?
    {
        if (item == null)
        {
            item = acquiredItem;
        }
    }
}
