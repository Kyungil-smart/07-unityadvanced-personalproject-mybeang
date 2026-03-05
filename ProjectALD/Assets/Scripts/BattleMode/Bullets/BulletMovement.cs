using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    private float _speed;
    private GameObject _target;
    public bool IsFire;
    private EnemyStatus _targetStatus;

    private void Update()
    {
        Movement();
        DestroySelf();
    }
    
    public void SetData(float speed, GameObject target)
    {
        _speed = speed;
        _target = target;
        _targetStatus = _target.GetComponent<EnemyStatus>();
    }
    
    private void DestroySelf()
    {   
        if (_targetStatus.isDead && gameObject.activeSelf)
        {
            Debug.Log($"{gameObject.name} 자멸");
            ObjectPoolManager.Instance.PushGameObject(gameObject);
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
                transform.position, _target.transform.position, _speed * Time.deltaTime);
        }
            
    }
}
