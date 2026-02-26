using System.Collections;
using UnityEngine;
public class BaseBelt : ObjectOnTile, IInteractableBeltPut, IBeltBehavior
{
    public Item item;
    public GameObject tail;
    public GameObject head;
    public bool IsConnectTail { get => (tail != null); }
    public bool IsConnectHead { get => (head != null); }
    
    private Direction _direction = Direction.East;
    private Transform _tf;
    private WaitForSeconds _waitOneSecond;
    
    private void Awake()
    {
        _tf = GetComponent<Transform>();
    }

    private void Start()
    {
        _waitOneSecond = new WaitForSeconds(1f);
    }

    private void Init()
    {
        // left tail, right head
    }
    
    public IEnumerator GetItem()
    {
        while (true)
        {
            tail?.GetComponent<IInteractableBeltGet>()?.InteractBeltGet(this);
            yield return _waitOneSecond;    
        }
        
    }

    public IEnumerator PutItem()
    {
        while (true)
        {
            head?.GetComponent<IInteractableBeltPut>()?.InteractBeltPut(this);
            yield return _waitOneSecond;
        }
    }

    public void Rotate()
    {
        DirectionUtil.Rotate(ref _direction);
        RotateImage();
        ConnectTailNHead();
    }

    private void RotateImage()
    {
        _tf.rotation = Quaternion.Euler(0, 0, _tf.rotation.eulerAngles.z - 90f);
    }

    private void ConnectTailNHead()
    {
        tail = null;
        head = null;
        
        Vector2Int pos = myTile.GridPos;
        Vector2Int tailPos;
        Vector2Int headPos;
        switch (_direction)
        {
            case Direction.West:
                tailPos = pos + DirectionUtil.ToAxis[Direction.East];
                headPos = pos + DirectionUtil.ToAxis[Direction.West];
                break;
            case Direction.North:
                tailPos = pos + DirectionUtil.ToAxis[Direction.South];
                headPos = pos + DirectionUtil.ToAxis[Direction.North];  
                break;
            case Direction.South:
                tailPos = pos + DirectionUtil.ToAxis[Direction.North];
                headPos = pos + DirectionUtil.ToAxis[Direction.South];
                break;
            default:
                tailPos = pos + DirectionUtil.ToAxis[Direction.West];
                headPos = pos + DirectionUtil.ToAxis[Direction.East];
                break;
        }
        tail = GridManager.Instance.GetObjectOnTile(tailPos);
        head = GridManager.Instance.GetObjectOnTile(headPos);
    }

    public void InteractBeltPut(BaseBelt baseBelt)
    {
        baseBelt.item = item;
        item = null;
    }
}
