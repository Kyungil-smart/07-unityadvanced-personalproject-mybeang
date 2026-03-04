using System.Collections;
using UnityEngine;

public class EnemyRangeAttack : MonoBehaviour, IAttackable
{
    private Animator _animator;
    private EnemyStatus status;
    private Ray2D ray;
    private GameObject target;
    private WaitForSeconds attackInterval = new WaitForSeconds(3.0f);
    private Coroutine attackCoroutine;
    [SerializeField] private GameObject bulletPrefab;
    private IBullet _iBullet;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void Start()
    {
        status = gameObject.GetComponent<EnemyStatus>();
    }
    
    private void Update()
    {
        CheckAttackRange();
    }

    private void CheckAttackRange()
    {
        ray = new Ray2D(transform.position, transform.right * -1);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, status.data.attackRange + 0.1f);
        if (hit.collider != null && hit.collider.tag.Contains("Wall"))
        {
            target = hit.collider.gameObject;
            if (attackCoroutine == null) 
                attackCoroutine = StartCoroutine(Attack());
        }
    }
    
    public IEnumerator Attack()
    {
        while (!status.isDead)
        {
            _animator.SetTrigger("AttackTrigger");
            
            // ToDo. Object Pool
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            
            _iBullet = bullet.GetComponent<IBullet>();
            _iBullet.SetTarget(target);
            _iBullet.Fire();
            yield return attackInterval;
        }
    }
}
