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
    public List<ConnectPoint> bothes = new();

    protected abstract void InitNumberOfConnectPoint();
    public abstract void PutOnTileHandler();
    public abstract void TakeOffTileHandler();

    public void PrintLog(string text) 
        => Debug.Log($"[{myTile?.GridPos.x},{myTile?.GridPos.y}]({gameObject.name}) {text}");
    
    public bool IsConnectable(GameObject neigbor, ConnectPoint cpoint)
    {   // 상대방이 연결 가능한 상태인지 확인하는 함수.
        PrintLog($"{cpoint.direction}) -> {neigbor.name}; 연결 가능 여부 확인 시작");
        if (neigbor == null) return false;
        // 벨트 액션이 가능한지 여부 확인.
        if (neigbor.GetComponent<IInteractableBeltGet>() == null && neigbor.GetComponent<IInteractableBeltPut>() == null)
            return false;
        PrintLog($"{cpoint.direction}) -> {neigbor.name}; 벨트 액션이 가능한지 여부 확인 완료.");
        ObjectOnTile neighborOot = neigbor.GetComponent<ObjectOnTile>();
        if (neighborOot == null) return false;
        // 상대방에게 나와 Interaction 을 위한 맺음이 가능한지 여부 질의
        PrintLog($"{cpoint.direction}) -> {neigbor.name}; 상대방에게 나와 Interaction 을 위한 맺음이 가능한지 여부 질의.");
        if (!neighborOot.IsConnectWith(gameObject, cpoint))
        {
            PrintLog($"{cpoint.direction}) -> {neigbor.name}; 연결 불가");
            return false;
        }
        PrintLog($"{cpoint.direction}) -> {neigbor.name}; 연결 가능");
        return true;
    }
    
    public bool IsConnectWith(GameObject neighbor, ConnectPoint neighborCpoint)
    {   // 상대방에게 질의를 하기 위한 함수.
        // 서로가 tail <-> head 로 물리며 각가의 방향이 반대방향으로 되어있는지 확인.
        // 질의 하여 상대방이 OK 하는 경우 상대방의 ConnectPoint 에도 나의 object 를 update 해주기 위함.
        // 여기서 neighbor 개념은 질의한, 즉 현제 제어중인 object. 
        // ToDo: 조금 더 이쁘게 확인할 수 있는 방법이 없을까?
        bool result;
        if (neighborCpoint.type == ConnectPointType.Tail)
        {
            PrintLog($"check Tail -> Head");
            result = ValidateNeighbor(neighbor, neighborCpoint, heads);
            if (!result)
            {
                PrintLog($"check Tail -> Both");
                result = ValidateNeighbor(neighbor, neighborCpoint, bothes);
            }
        }
        else if (neighborCpoint.type == ConnectPointType.Head)
        {
            PrintLog($"check Head -> Tail");
            result = ValidateNeighbor(neighbor, neighborCpoint, tails);
            if (!result)
            {
                PrintLog($"check Head -> Both");
                result = ValidateNeighbor(neighbor, neighborCpoint, bothes);
            }
        }
        else
        {  // Both Type
            PrintLog($"check Both -> Both");
            result = ValidateNeighbor(neighbor, neighborCpoint, bothes); 
            if (!result)
            {
                PrintLog($"check Both -> Head");
                result = ValidateNeighbor(neighbor, neighborCpoint, heads);
                if (!result)
                {
                    PrintLog($"check Both -> Tail");
                    result = ValidateNeighbor(neighbor, neighborCpoint, tails);
                }
            }
        }
        return result;
    }

    private bool ValidateNeighbor(GameObject neighbor, ConnectPoint neighborCpoint, List<ConnectPoint> cpoints)
    {
        PrintLog($"validate neighbor => {neighbor.name}.{neighborCpoint.direction} // {cpoints.Count}");
        foreach (var cpoint in cpoints)
        {
            PrintLog($"validate neighbor => {neighbor.name}.{neighborCpoint.direction} vs {cpoint.direction}");
            if (DirectionUtil.IsOppositeDirection(cpoint.direction, neighborCpoint.direction))
            {   // validate 과정에서 connect 발생. 좋은지 안좋은지 잘 모르겠음....
                cpoint.neighbor = neighbor;
                PrintLog($"validate neighbor => {neighbor.name}.{neighborCpoint.direction} vs {cpoint.direction} ; 성공");
                return true;
            }
            PrintLog($"validate neighbor => {neighbor.name}.{neighborCpoint.direction} vs {cpoint.direction} ; 실패");
        }
        return false;
    }
    
    public void ConnectToNeighbor(ConnectPoint cpoint)
    {
        cpoint.neighbor = null;
        // Connect Point 가 위치한 방향의 앞쪽 Grid 에 Object 가 있는지 확인.
        Vector2Int neighborPos = myTile.GridPos + DirectionUtil.ToAxis[cpoint.direction];
        GameObject neighbor = GridManager.Instance.GetObjectOnTile(neighborPos);
        
        if (neighbor == null) return;
        
        // neighor 가 벽일 경우 대비하기
        if (neighbor.tag.Contains("Wall"))
        {
            Wall wall = neighbor.GetComponent<Wall>();
            neighbor = wall.DefenceUnit;
        }
        
        if (IsConnectable(neighbor, cpoint))
        {
            cpoint.neighbor = neighbor;
            PrintLog($"{neighbor.name} 와(과) 정상적으로 연결되었음");
        }
        else
        {
            PrintLog("상대방과 연결에 실패하였음.");    
        }
    }
    
    public void SetLayerPriority(int priority)
    {
        if (_spriteRenderer != null) 
            _spriteRenderer.sortingOrder = priority;
    }

    public void ClearAllConnectPoints()
    {
        foreach (var cpoint in bothes) cpoint.Clear();
        foreach (var cpoint in heads) cpoint.Clear();
        foreach (var cpoint in tails) cpoint.Clear();
        
        bothes.Clear();
        heads.Clear();
        tails.Clear();
    }
}
