using UnityEngine;
public class BaseBelt : ObjectOnTile, IInteractableBeltPut
{
    public Item item;
    public GameObject tail;
    public GameObject head;
    public bool IsConnectTail { get => (tail != null); }
    public bool IsConnectHead { get => (head != null); }

    private Transform _tf;
    
    private void Awake()
    {
        _tf = GetComponent<Transform>();
    }

    private void Start()
    {
        
    }

    private void Init()
    {
        // left tail, right head
    }
    
    public void GetItem()
    {
        tail?.GetComponent<IInteractableBeltGet>()?.InteractBeltGet(this);
    }

    public void PutItem()
    {
        tail?.GetComponent<IInteractableBeltPut>()?.InteractBeltPut(this);
    }

    public void Rotate()
    {
        // rotate image
        _tf.rotation = Quaternion.Euler(0, 0, _tf.rotation.eulerAngles.z - 90f);
        
        // insert new tail
        tail = GridManager.Instance.GetObjectOnTile(myTile.GridPos.x, myTile.GridPos.y);

        // insert new haad
    }

    public void InteractBeltPut(BaseBelt baseBelt)
    {
        
    }
}
