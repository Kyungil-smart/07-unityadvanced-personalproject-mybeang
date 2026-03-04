using System;
using System.Collections;
using UnityEngine;

public class Tower : ObjectOnTile, IMovableBuilding, IAttackable, IInteractableBeltPut, ISellable
{
    private Coroutine _attackCoroutine;
    private GameObject _target;
    public BulletBox bulletBox;
    private WaitForSeconds attackInterval = new WaitForSeconds(1f);
    private BulletData bulletData;
    private TowerData data;
    private string _state;
    private WaitForSeconds animWait = new WaitForSeconds(0.25f);
    [SerializeField] private LayerMask enemyLayer;
    public ConnectPoint tail
    {
        get
        {
            if (tails.Count == 0) return null;
            return tails[0];
        }
        set
        {
            if (tails.Count == 0) tails.Add(value);
            else tails[0] = value;
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _state = "IsNotWorking";
    }
    
    private void OnEnable()
    {
        InitNumberOfConnectPoint();
    }

    private void Update()
    {
        switch (_state)
        {
            case "IsFinding":
                FindEnemy();
                if (_attackCoroutine != null)
                {
                    StopCoroutine(_attackCoroutine);
                    _attackCoroutine = null;
                }
                break;
            case "IsAttacking":
                if (_attackCoroutine == null)
                    _attackCoroutine = StartCoroutine(Attack());
                break;
        }
    }

    private float ApplyDamageMultipler(float damage)
    {
        return PlayerStatusManager.Instance.damageMultiplier * damage;;
    }
    
    public IEnumerator Attack()
    {
        PrintLog("공격을 시작합니다.");
        while (_target != null)
        {
            if (bulletBox != null && bulletBox.GetBulletCount() > 0)
            {
                _animator.SetTrigger("IsAttacking");
                GameObject bullet = bulletBox.GetBullet();
                IBullet iBullet = bullet.GetComponent<IBullet>();
                PrintLog($"{bullet.name} 을 하나 꺼냄. IBullet: {iBullet}");
                iBullet.SetTarget(_target);
                iBullet.SetDamage(ApplyDamageMultipler(bulletData.damage));
                bullet.transform.position = transform.position;
                PrintLog($"{bullet.name} 데이터 갱신");
                yield return animWait;
                PrintLog($"{bullet.name} 을 쏜다");
                iBullet.Fire();
            } 
            else if (bulletBox != null && bulletBox.GetBulletCount() == 0)
            {
                PrintLog("탄약이 모두 소비되었습니다.");
                ObjectPoolManager.Instance.PushGameObject(bulletBox.gameObject);
                bulletBox = null;
            }
            yield return attackInterval;
        }
        PrintLog("타겟이 소멸하였습니다.");
        _state = "IsFinding";
    }

    private void FindEnemy()
    {
        _animator.SetTrigger("IsIdle");
        if (_target == null && bulletData != null)
        {
            PrintLog($"적을 찾습니다. 범위: {bulletData.range}");
            Collider2D target = Physics2D.OverlapCircle(transform.position, bulletData.range, enemyLayer);
            if (target != null)
            {
                PrintLog("적 포착!");
                _state = "IsAttacking";
                _target = target.gameObject;
            }
        }
    }

    protected override void InitNumberOfConnectPoint()
    {
        tails.Add(new ConnectPoint(ConnectPointType.Tail, Direction.West, null, gameObject));
    }

    public override void PutOnTileHandler()
    {
        LoadTowerData();
        ConnectToNeighbor(tail);
        _state = "IsFinding";
        PrintLog($"PutOnTileHandler. Now State is {_state}");
    }

    private void LoadTowerData()
    {
        data = DataManager.Instance.towerData["TowerSO"];
    }

    public override void TakeOffTileHandler()
    { }

    public void InteractBeltPut(Item acquiredItem)
    {
        if (bulletBox == null)
        {
            PrintLog("탄약 수급");
            bulletBox = acquiredItem as BulletBox;
            bulletData = bulletBox.data;
            attackInterval = new WaitForSeconds(bulletData.attackRate);
        }
    }

    public void SellSelf()
    {
        PlayerStatusManager.Instance.EarnGold(DataManager.Instance.buildCostData["Tower"]);
        ClearAllConnectPoints();
        ObjectPoolManager.Instance.PushGameObject(gameObject);
    }
}
