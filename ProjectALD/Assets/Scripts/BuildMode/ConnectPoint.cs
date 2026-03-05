using System.Collections.Generic;
using UnityEngine;

public class ConnectPoint
{
    public ConnectPointType type;
    public Direction direction;
    private GameObject _attachedObject;
    private GameObject _neighbor;
    public GameObject neighbor
    {
        get { return _neighbor; }
        set
        {
            _neighbor = value;
            putToNeighbor = _neighbor?.GetComponent<IInteractableBeltPut>();
            getFromNeighbor = _neighbor?.GetComponent<IInteractableBeltGet>();
        }
    }
    public IInteractableBeltPut putToNeighbor;
    public IInteractableBeltGet getFromNeighbor;

    public ConnectPoint(ConnectPointType type, Direction direction, GameObject neighbor, GameObject attachedObject)
    {
        this.type = type;
        this.direction = direction;
        this.neighbor = neighbor;
        _attachedObject = attachedObject;
    }
    
    public void RotateConnectPoint() => DirectionUtil.Rotate(ref direction);

    public void FlipConnectPoint() => DirectionUtil.Flip(ref direction);

    private void Disconnect(List<ConnectPoint> connectPoints)
    {
        foreach (var cp in connectPoints)
        {
            if (cp.neighbor == _attachedObject)
            {
                Debug.Log($"{_attachedObject.name} {direction} cp say: Disconnect to {cp.neighbor.name}");
                cp.neighbor = null;
                return;
            }
        }
    }
    
    public void Clear()
    {
        if (_neighbor != null)
        {
            ObjectOnTile nObj = neighbor.GetComponent<ObjectOnTile>();
            if (nObj != null)
            {
                Disconnect(nObj.heads);
                Disconnect(nObj.tails);
                Disconnect(nObj.bothes);
            }
            neighbor = null;
        }
    }
}
