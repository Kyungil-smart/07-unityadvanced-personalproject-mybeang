using UnityEngine;

public class Wall : ObjectOnTile
{
    private GameObject _tower;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void InitNumberOfConnectPoint()
    {
    }

    public override void PutOnTileHandler()
    {
    }
}
