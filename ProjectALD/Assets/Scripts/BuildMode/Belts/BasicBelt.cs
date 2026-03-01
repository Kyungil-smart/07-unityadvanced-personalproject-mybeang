using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BasicBelt : ObjectOnTile, IBelt, IInteractableBeltPut, IBeltBehavior, IMovableBuilding, IRotatable
{
    public Item item { get; set; }
    public SpriteRenderer subSpriteRenderer;
    public GameObject iconPrefab;
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
        tails.Add(new(ConnectPointType.Tail, Direction.East, null));
        heads.Add(new(ConnectPointType.Head, Direction.West, null));
    }

    public override void PutOnTileHandler()
    {
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
        _deliverItemRoutine = StartCoroutine(DeliverItem());
    }

    private void StopOperation()
    {
        if (_deliverItemRoutine != null) StopCoroutine(_deliverItemRoutine);
        _deliverItemRoutine = null;
    }

    public IEnumerator DeliverItem()
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
        if (item == null)
        {
            tail.getFromNeighbor?.InteractBeltGet(this);
        }
        if (item != null)
        {
            subSpriteRenderer.sprite = item.spriteRenderer.sprite;
            subSpriteRenderer.color = item.spriteRenderer.color;
            if (item.itemType == ItemType.BulletBox)
            {
                iconPrefab.transform.localScale = new Vector3(0.4f, 0.5f, 1);
            }
            else
            {
                iconPrefab.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            PrintLog("아이템이없다고?");
            subSpriteRenderer.sprite = null;
            subSpriteRenderer.color = Color.white;
        }
    }

    public void PutItem()
    {
        head.putToNeighbor?.InteractBeltPut(this);
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
        RotateConnectPoint(head);
        RotateConnectPoint(tail);
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

    private void RotateConnectPoint(ConnectPoint cpoint) => DirectionUtil.Rotate(ref cpoint.direction);
    
    public void InteractBeltPut<T>(T belt) where T : ObjectOnTile, IBelt
    {
        if (belt.item != null && item == null)
        {   
            item = belt.item;
            belt.item = null;
        }
    }
}
