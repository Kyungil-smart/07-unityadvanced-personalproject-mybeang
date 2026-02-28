using UnityEngine;

public class ConnectPoint
{
    public ConnectPointType type;
    public Direction direction;
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

    public ConnectPoint(ConnectPointType type, Direction direction, GameObject neighbor)
    {
        this.type = type;
        this.direction = direction;
        this.neighbor = neighbor;
    }
}
