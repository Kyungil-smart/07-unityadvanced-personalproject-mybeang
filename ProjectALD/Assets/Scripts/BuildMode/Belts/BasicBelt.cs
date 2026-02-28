using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BasicBelt : ObjectOnTile, IInteractableBeltPut, IBeltBehavior, IMovableBuilding, IRotatable
{
    public GameObject item;
    public SpriteRenderer subSpriteRenderer;
    private WaitForSeconds _waitOneSecond;
    private Sprite _cacheItemSprite;
    private Color _cacheItemColor = Color.white;

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
        _waitOneSecond = new WaitForSeconds(1f);
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
            yield return _waitOneSecond;
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
            if (_cacheItemSprite == null && _cacheItemColor == Color.white)
            {
                PrintLog("아직 뭐가 없으니까 그림 그릴거 내놔");
                SpriteRenderer itemSR = item.GetComponent<SpriteRenderer>();   
                _cacheItemSprite = itemSR.sprite;
                _cacheItemColor = itemSR.color;
                subSpriteRenderer.sprite = itemSR.sprite;
                subSpriteRenderer.color = itemSR.color;
            }
            else
            {
                PrintLog("캐싱된거써");
                subSpriteRenderer.sprite = _cacheItemSprite;
                subSpriteRenderer.color = _cacheItemColor;
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
        item = null;
        RotateImage();
        RotateConnectPoint(head);
        RotateConnectPoint(tail);
        if (myTile != null)
        {
            ConnectToNeighbor(head);
            ConnectToNeighbor(tail);    
        }
    }

    private void RotateImage()
    {
        _animator?.SetTrigger("Rotate");
    }

    private void RotateConnectPoint(ConnectPoint cpoint) => DirectionUtil.Rotate(ref cpoint.direction);
    
    private void ConnectToNeighbor(ConnectPoint cpoint)
    {
        cpoint.neighbor = null;
        Vector2Int neighborPos = myTile.GridPos + DirectionUtil.ToAxis[cpoint.direction];
        GameObject neighbor = GridManager.Instance.GetObjectOnTile(neighborPos);
        if (IsConnectable(neighbor, cpoint))
        {
            cpoint.neighbor = neighbor;
        }
    }

    public void InteractBeltPut(BasicBelt basicBelt)
    {
        PrintLog("줄거 있다며?");
        if (basicBelt.item != null && item == null)
        {
            PrintLog($"내놓지그래?");
            item = basicBelt.item;
            basicBelt.item = null;
        }
    }
}
