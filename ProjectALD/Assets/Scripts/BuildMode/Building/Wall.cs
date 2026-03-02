using UnityEngine;

public class Wall : ObjectOnTile, IPlacable, IDamagable
{
    private GameObject _tower;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void InitNumberOfConnectPoint() { }
    public override void PutOnTileHandler() { }
    public override void TakeOffTileHandler() { }
    public void TakeDamage(float damage)
    {
        GameManager.Instance.currentHp -= (int)damage;  // ToDo. 추후 변경
    }
}
