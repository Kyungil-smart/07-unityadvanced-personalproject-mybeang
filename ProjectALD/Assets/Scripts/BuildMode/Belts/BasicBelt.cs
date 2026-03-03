using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BasicBelt : ObjectOnTile, IBelt, IInteractableBeltPut, IBeltBehavior, IMovableBuilding, IRotatable
{
    public Item item { get; set; }
    public SpriteRenderer subSpriteRenderer;
    public GameObject iconPrefab;
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
            UpdateSubItemIcon();
        }
    }

    private void UpdateSubItemIcon()
    {
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
            subSpriteRenderer.sprite = null;
            subSpriteRenderer.color = Color.white;
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
        head.RotateConnectPoint();
        tail.RotateConnectPoint();
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
    
    public void InteractBeltPut(Item acquiredItem)
    {
        if (item == null)
        {
            item = acquiredItem;
            UpdateSubItemIcon();
        }
    }
}
