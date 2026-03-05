using UnityEngine;

public interface IBullet
{
    public void SetDamage(float damage);
    public void SetTarget(GameObject target);
    public void Fire();
    public void Attack();
}
