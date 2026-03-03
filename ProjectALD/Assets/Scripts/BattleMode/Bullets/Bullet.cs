using UnityEngine;

public class Bullet : Item, IBullet
{
    public BulletData data;
    private void Start()
    {
        data = LoadBulletData("BulletSO");
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
