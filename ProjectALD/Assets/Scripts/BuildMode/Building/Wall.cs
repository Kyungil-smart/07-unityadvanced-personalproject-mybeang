using UnityEngine;

public class Wall : ObjectOnTile, IPlacable, IDamagable
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
        GameManager.Instance.currentHp -= (int)damage;  // ToDo. 추후 변경
    }
    
    private void TowerSetOn()
    {
        int priority = 100 - myTile.GridPos.y;
        Tower tower = _defenceUnit.GetComponent<Tower>();
        _defenceUnit.transform.position = transform.position;
        tower.SetLayerPriority(priority);
        tower.myTile = myTile;
        tower.PutOnTileHandler();
    }
}
