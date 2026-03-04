using System.Collections;
using UnityEngine;

public class CurvBelt : ObjectOnTile, IBelt, IInteractableBeltPut, IBeltBehavior, IMovableBuilding, IRotatable, IFlip, ISellable
{
    public Item item { get; set; }
    public Canvas helpCanvas;
    private WaitForSeconds _deliveryInterval;

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
    
    private Coroutine _deliverItemRoutine;

    private IInteractableBeltGet _getFromNeighbor;
    private IInteractableBeltPut _putToNeighbor;
    
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
        tails.Add(new(ConnectPointType.Tail, Direction.South, null, gameObject));
        heads.Add(new(ConnectPointType.Head, Direction.East, null, gameObject));
    }

    public override void PutOnTileHandler()
    {
        helpCanvas.gameObject.SetActive(false);
        _spriteRenderer.sortingLayerName = "Belt";
        ConnectToNeighbor(head);
        ConnectToNeighbor(tail);
    }

    public override void TakeOffTileHandler()
    {
        head.neighbor = null;
        tail.neighbor = null;
    }

    private void StartOperation()
    {
        PrintLog("Operation Start");
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
            PutItem();
            GetItem();
            yield return _deliveryInterval;
        }
    }

    public void GetItem()
    {
        if (item == null && tail.getFromNeighbor != null)
        {
            item = tail.getFromNeighbor.InteractBeltGet();
        }
    }

    public void PutItem()
    {
        if (item != null && head.putToNeighbor != null)
        {
            head.putToNeighbor.InteractBeltPut(item);
            item = null;
        }
    }

    private bool IsConnected()
    {
        if (heads.Count == 0 || tails.Count == 0) return false;
        if (head.neighbor == null && tail.neighbor == null) return false;
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
        PrintLog($"Before Rotate: h;{head.direction} t;{tail.direction}");
        head.RotateConnectPoint();
        tail.RotateConnectPoint();
        PrintLog($"After Rotate: h;{head.direction} t;{tail.direction}");
        if (myTile != null)
        {
            // 타일 위에 이미 지어진 것이면 다시 Connection 시도.
            ConnectToNeighbor(head);
            ConnectToNeighbor(tail);    
        }
    }

    private void RotateImage()
    {
        _animator?.SetTrigger("Rotate");
    }

    public void Flip()
    {
        // e->w w-> 만 신경쓰면 된다!!
        // 동작 멈춤
        StopOperation();
        // 아이템 비움
        item = null;
        // 이미지 변경
        FlipImage();
        
        // Connect Point 변경
        PrintLog($"Before Flip: h;{head.direction} t;{tail.direction}");
        if (head.direction == Direction.East || head.direction == Direction.West)
            head.FlipConnectPoint();
        else if (tail.direction == Direction.East || tail.direction == Direction.West)
            tail.FlipConnectPoint();
        PrintLog($"After Flip: h;{head.direction} t;{tail.direction}");
        if (myTile != null)
        {
            // 타일 위에 이미 지어진 것이면 다시 Connection 시도.
            ConnectToNeighbor(head);
            ConnectToNeighbor(tail);
        }
    }

    private void FlipImage()
    {
        _animator.SetTrigger("Flip");
    }
    
    public void InteractBeltPut(Item acquiredItem)
    {
        if (item == null)
        {
            item = acquiredItem;
        }
    }

    public void SellSelf()
    {
        ClearAllConnectPoints();
        // ToDo. ObjectPool
        Destroy(gameObject);
    }
}
