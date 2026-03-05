using UnityEngine;

// ToDo. Not Implement
public class BulletIce : Item, IBullet
{
    public BulletData data;
    private void Start()
    {
        data = LoadBulletData("BulletIceSO");
    }

    public void SetDamage(float damage)
    {
        throw new System.NotImplementedException();
    }

    public void SetTarget(GameObject target)
    {
        throw new System.NotImplementedException();
    }

    public void Fire()
    {
        throw new System.NotImplementedException();
    }

    public void Movement()
    {
        throw new System.NotImplementedException();
    }

    public void Attack()
    {
        throw new System.NotImplementedException();
    }
}
