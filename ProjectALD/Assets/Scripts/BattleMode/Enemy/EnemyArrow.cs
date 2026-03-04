using System.Collections;
using UnityEngine;


public class EnemyArrow : Item, IBullet
{
    private GameObject _target;
    private BulletMovement _movement;
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;
    
    private void Awake()
    {
        // 추후 다른 MonsterSO 도 받을 수 있도록 변경 필요.
        MonsterData data = DataManager.Instance.monsterData["MonsterSO_04"];
        _damage = data.damage;
        _speed = 10f;
        _movement = GetComponent<BulletMovement>();
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {   // 해당 타겟에 맞거나 중간에 다른 몹과 부딧히면 공격
        if (collision.gameObject.tag.Contains("Wall"))
        {
            Debug.Log($"{gameObject.name}, {_target.name} 에게 적중");
            _target = collision.gameObject;
            Attack();
            // ToDo. Object Pool
            Destroy(gameObject);
        }
    }

    public void SetDamage(float damage) {}

    public void SetTarget(GameObject target)
    {
        _target = target;
        _movement.SetData(_speed, target);
    }

    public void Fire()
    {
        gameObject.SetActive(true);
        _movement.IsFire = true;
    }

    public void Attack()
    {   
        Debug.Log($"{gameObject.name}, {_target.name} 에게 {_damage}의 데미지");
        // _target?.GetComponent<IDamagable>()?.TakeDamage(_damage, DamageType.None);
    }
}
