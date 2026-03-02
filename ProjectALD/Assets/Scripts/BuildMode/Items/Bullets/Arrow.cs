using System.Collections;
using UnityEngine;

public class Arrow : Item, IBullet
{
    public BulletData data;
    private GameObject _target;
    public bool IsFire;
    
    private void Start()
    {
        data = LoadBulletData("ArrowSO");
    }

    private void Update()
    {
        Movement();
        DestroySelf();
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {   // 해당 타겟에 맞거나 중간에 다른 몹과 부딧히면 공격
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log($"{gameObject.name}, {_target.name} 에게 적중");
            _target = collision.gameObject;
            Attack();
        }
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }

    public void Fire()
    {
        gameObject.SetActive(true);
        IsFire = true;
    }

    private void DestroySelf()
    {   // ToDo. Object Pool
        if (_target == null && gameObject.activeSelf)
        {
            Debug.Log($"{gameObject.name} 자멸");
            Destroy(gameObject);
        }
    }

    private void LookAtTarget()
    {
        if (_target == null) return;
        Vector3 direction = _target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    public void Movement()
    {   // 타겟위치 까지 움직임
        if (_target != null && IsFire)
        {
            LookAtTarget();
            transform.position = Vector3.MoveTowards(
                transform.position, _target.transform.position, data.speed * Time.deltaTime);
        }
            
    }

    public void Attack()
    {   // 데미지를 입히자. 추후 데미지 계산식 추가 필요.
        Debug.Log($"{gameObject.name}, {_target.name} 에게 {data.damage}의 데미지");
        _target?.GetComponent<IDamagable>()?.TakeDamage(data.damage, data.damageTypes[1]);
        Destroy(gameObject);
    }
}
