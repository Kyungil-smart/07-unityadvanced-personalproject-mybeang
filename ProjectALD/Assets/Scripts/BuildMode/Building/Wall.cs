using UnityEngine;

public class Wall : ObjectOnTile, IPlacable, IDamagable, ISellable
{
    private GameObject _defenceUnit;
    public GameObject DefenceUnit
    {
        get
        {
            return _defenceUnit;
        }
        set 
        {
            _defenceUnit = value;
            TowerSetOn();
        }
    }
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void InitNumberOfConnectPoint() { }
    public override void PutOnTileHandler() { }
    public override void TakeOffTileHandler() { }
    public void TakeDamage(float damage, DamageType damageType)
    {
        PlayerStatusManager.Instance.currentHp -= (int)damage;  // ToDo. 추후 변경
    }
    
    private void TowerSetOn()
    {
        PrintLog("Tower set on");
        int priority = 100 - myTile.GridPos.y;
        Tower tower = _defenceUnit.GetComponent<Tower>();
        _defenceUnit.transform.position = transform.position;
        tower.SetLayerPriority(priority);
        tower.myTile = myTile;
        tower.PutOnTileHandler();
    }

    public void SellSelf()
    {
        DefenceUnit.GetComponent<Tower>()?.SellSelf();
    }
}
