using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ObjectOnTile : MonoBehaviour, IIsConnectableWith
{
    public Tile myTile;
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;

    public List<ConnectPoint> tails = new();
    public List<ConnectPoint> heads = new();

    protected abstract void InitNumberOfConnectPoint();
    public abstract void PutOnTileHandler();
    public abstract void TakeOffTileHandler();

    public void PrintLog(string text) 
        => Debug.Log($"[{myTile.GridPos.x},{myTile.GridPos.y}]({gameObject.name}) {text}");
    
    public bool IsConnectable(GameObject neigbor, ConnectPoint cpoint)
    {   // 상대방이 연결 가능한
        if (neigbor == null) return false;
        if (neigbor.GetComponent<IInteractableBeltGet>() == null && neigbor.GetComponent<IInteractableBeltPut>() == null)
            return false;
        ObjectOnTile oot = neigbor.GetComponent<ObjectOnTile>();
        if (oot == null) return false;
        if (!oot.IsConnectWith(gameObject, cpoint))
        {
            return false;
        }
        return true;
    }
    
    public bool IsConnectWith(GameObject neighbor, ConnectPoint neighborCpoint)
    {   // 서로가 tail <-> head 로 물리며 각가의 방향이 반대방향으로 되어있는지 확인.
        // 상대방에게 질의를 하기 위한 함수.
        // 질의 하여 상대방이 OK 하는 경우 상대방의 ConnectPoint 에도 나의 object 를 update 해주기 위함.
        if (neighborCpoint.type == ConnectPointType.Tail)
            return ValidateNeighbor(neighbor, neighborCpoint, heads);
        return ValidateNeighbor(neighbor, neighborCpoint, tails);
    }

    private bool ValidateNeighbor(GameObject neighbor, ConnectPoint neighborCpoint, List<ConnectPoint> cpoints)
    {
        foreach (var cpoint in cpoints)
        {
            if (DirectionUtil.IsOppositeDirection(cpoint.direction, neighborCpoint.direction))
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
