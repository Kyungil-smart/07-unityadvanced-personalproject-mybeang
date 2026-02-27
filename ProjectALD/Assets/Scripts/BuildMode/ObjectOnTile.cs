using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ObjectOnTile : MonoBehaviour, IIsConnectableWith
{
    public Tile myTile;
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;

    public List<ConnectPoint> tails;
    public List<ConnectPoint> heads;

    protected abstract void InitNumberOfConnectPoint();
    public abstract void PutOnTileHandler();
    
    public bool IsConnectWith(GameObject neighbor, ConnectPoint neighborCpoint)
    {   // Neighbor ask to me the connection is right?
        if (neighborCpoint.type == ConnectPointType.Tail)
            return ValidateNeighbor(neighbor, heads);
        return ValidateNeighbor(neighbor, tails);
    }

    private bool ValidateNeighbor(GameObject neighbor, List<ConnectPoint> cpoints)
    {
        foreach (var cpoint in cpoints)
        {
            if (DirectionUtil.IsOppositeDirection(cpoint.direction, cpoint.direction))
            {   // validate 과정에서 connect 발생. 좋은지 안좋은지 잘 모르겠음....
                cpoint.neighbor = neighbor;
                return true;
            }
        }
        return false;
    }
    
    public void SetLayerPriority(int priority)
    {
        if (_spriteRenderer != null) 
            _spriteRenderer.sortingOrder = priority;
    }
}
