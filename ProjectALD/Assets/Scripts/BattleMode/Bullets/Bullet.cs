using UnityEngine;

public class Bullet : Item, IBullet
{
    public BulletData data;
    private GameObject _target;
    private BulletMovement _movement;
    
    private void Start()
    {
        data = LoadBulletData("BulletSO");
        _movement = GetComponent<BulletMovement>();
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {   // 해당 타겟에 맞거나 중간에 다른 몹과 부딧히면 공격
        if (collision.gameObject.tag.Contains("Enemy"))
        {
            Debug.Log($"{gameObject.name}, {_target.name} 에게 적중");
            _target = collision.gameObject;
            Attack();
            // ToDo. Object Pool
            Destroy(gameObject);
        }
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
        _movement.SetData(data, target);
    }

    public void Fire()
    {
        gameObject.SetActive(true);
        _movement.IsFire = true;
    }

    public void Attack()
    {   // 데미지를 입히자. 추후 데미지 계산식 추가 필요.
        Debug.Log($"{gameObject.name}, {_target.name} 에게 {data.damage}의 데미지");
        _target?.GetComponent<IDamagable>()?.TakeDamage(data.damage, data.damageTypes[1]);
        
    }
}
