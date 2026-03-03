using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public BulletData data;
    private GameObject _target;
    public bool IsFire;

    private void Update()
    {
        Movement();
        DestroySelf();
    }
    
    public void SetData(BulletData newData, GameObject target)
    {
        data = newData;
        _target = target;
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
}
