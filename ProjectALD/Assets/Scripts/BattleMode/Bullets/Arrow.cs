using System.Collections;
using UnityEngine;

public class Arrow : Item, IBullet
{
    public BulletData data;
    private GameObject _target;
    private BulletMovement _movement;
    private float _damage;
    private float _effortRange = 1f;
    [SerializeField] LayerMask _targetLayer;

    private void Awake()
    {
        data = LoadBulletData("ArrowSO");
        _movement = GetComponent<BulletMovement>();
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {   // 해당 타겟에 맞거나 중간에 다른 몹과 부딧히면 공격
        if (collision.gameObject.tag.Contains("Enemy"))
        {
            _target = collision.gameObject;
            Debug.Log($"{gameObject.name}, {_target.name} 에게 적중");
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _effortRange, _targetLayer);
            foreach (Collider2D col in colliders)
            {
                _target = col.gameObject;
                Attack();
            }   
            ObjectPoolManager.Instance.PushGameObject(gameObject);
        }
    }

    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
        Debug.Log($" >>> SetTarget : {data} {target}");
        _movement.SetData(data.speed, target);
    }

    public void Fire()
    {
        gameObject.SetActive(true);
        _movement.IsFire = true;
        StartCoroutine(Drop());
    }

    public void Attack()
    {   
        Debug.Log($"{gameObject.name}, {_target.name} 에게 {_damage}의 데미지");
        _target?.GetComponent<IDamagable>()?.TakeDamage(_damage, data.damageTypes[1]);
    }

    private IEnumerator Drop()
    {
        // Fire 된 후 3초 뒤에 자동 수납
        yield return new WaitForSeconds(3f);
        ObjectPoolManager.Instance.PushGameObject(gameObject);
    }
}
