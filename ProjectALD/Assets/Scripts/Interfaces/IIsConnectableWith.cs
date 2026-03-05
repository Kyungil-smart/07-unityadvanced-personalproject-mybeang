using UnityEngine;

public interface IIsConnectableWith
{   // Neighbor ask to me the connection is right?
    public bool IsConnectWith(GameObject neighbor, ConnectPoint neighborCpoint);
}