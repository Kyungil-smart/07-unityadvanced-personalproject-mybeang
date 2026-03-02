using UnityEngine;

public interface IBullet
{
    public void SetTarget(GameObject target);
    public void Fire();
    public void Movement();
    public void Attack();
}
