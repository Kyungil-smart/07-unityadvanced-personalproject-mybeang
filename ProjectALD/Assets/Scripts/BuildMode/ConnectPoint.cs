using UnityEngine;

public class ConnectPoint
{
    public ConnectPointType type;
    public Direction direction;
    public GameObject neighbor;

    public ConnectPoint(ConnectPointType type, Direction direction, GameObject neighbor)
    {
        this.type = type;
        this.direction = direction;
        this.neighbor = neighbor;
    }
}
