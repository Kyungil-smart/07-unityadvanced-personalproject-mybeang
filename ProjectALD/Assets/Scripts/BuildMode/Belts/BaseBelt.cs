using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BaseBelt : ObjectOnTile, IInteractableBeltPut, IBeltBehavior
{
    public GameObject item;
    
    private Direction _direction = Direction.East;
    private Transform _tf;
    private WaitForSeconds _waitOneSecond;
    
    public ConnectPoint tail { get => tails[0]; set => tails[0] = value; }
    public ConnectPoint head { get => heads[0]; set => heads[0] = value; }
    
    private Coroutine _getItemRoutine;
    private Coroutine _putItemRoutine;

    private IInteractableBeltGet _getFromNeighbor;
    private IInteractableBeltPut _putToNeighbor;
    
    private void Awake()
    {
        _tf = GetComponent<Transform>();
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
        if (IsConnected() && _getItemRoutine == null && _putItemRoutine == null) 
            StartOpearation();
        else 
            StopOpearation();
    }
    
    protected override void InitNumberOfConnectPoint()
    {
        tails.Add(new(ConnectPointType.Tail, Direction.East, null));
        heads.Add(new(ConnectPointType.Head, Direction.West, null));
    }

    public override void PutOnTileHandler()
    {
        
    }

    private void StartOpearation()
    {
        _getItemRoutine = StartCoroutine(GetItem());
        _putItemRoutine = StartCoroutine(PutItem());
    }

    private void StopOpearation()
    {
        StopCoroutine(_getItemRoutine);
        StopCoroutine(_putItemRoutine);
        _getItemRoutine = null;
        _putItemRoutine = null;
    }
    
    public IEnumerator GetItem()
    {
        while (true)
        {
            _getFromNeighbor?.InteractBeltGet(this);
            yield return _waitOneSecond;    
        }
    }

    public IEnumerator PutItem()
    {
        while (true)
        {
            _putToNeighbor?.InteractBeltPut(this);
            yield return _waitOneSecond;
        }
    }

    private bool IsConnected()
    {
        if (head.neighbor == null || tail.neighbor == null) return false;
        return true;
    }

    public void Rotate()
    {
        DirectionUtil.Rotate(ref _direction);
        RotateImage();
        ConnectToNeighbor(head);
        ConnectToNeighbor(tail);
    }

    private void RotateImage()
    {
        _tf.rotation = Quaternion.Euler(0, 0, _tf.rotation.eulerAngles.z - 90f);
        // ToDo: spriterenderer 변경 필요
        // ToDO: animator 변경 필요
    }

    private void ConnectToNeighbor(ConnectPoint cpoint)
    {
        cpoint.neighbor = null;
        Vector2Int neighborPos = myTile.GridPos + DirectionUtil.ToAxis[cpoint.direction];
        GameObject neighbor = GridManager.Instance.GetObjectOnTile(neighborPos);
        if (IsConnectable(neighbor, cpoint)) cpoint.neighbor = neighbor;
        if (cpoint.type == ConnectPointType.Tail)
            _getFromNeighbor = cpoint.neighbor?.GetComponent<IInteractableBeltGet>();
        else
            _putToNeighbor = cpoint.neighbor?.GetComponent<IInteractableBeltPut>();
    }
    
    private bool IsConnectable(GameObject neigbor, ConnectPoint cpoint)
    {
        if (neigbor == null) return false;
        if (neigbor.GetComponent<IInteractableBeltPut>() == null 
            & neigbor.GetComponent<IInteractableBeltPut>() == null)
        { return false; }

        ObjectOnTile oot = neigbor.GetComponent<ObjectOnTile>();
        if (oot == null) return false;
        if (!oot.IsConnectWith(gameObject, cpoint)) return false;
        return true;
    }

    public void InteractBeltPut(BaseBelt baseBelt)
    {
        if (item == null) return;
        if (baseBelt.item == null)
        {
            baseBelt.item = item;
            item = null;
        }
    }
}
