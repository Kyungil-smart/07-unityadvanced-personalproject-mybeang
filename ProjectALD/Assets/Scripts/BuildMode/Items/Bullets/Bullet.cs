using UnityEngine;

public class Bullet : Item
{
    public BulletData data;
    private void Start()
    {
        data = LoadBulletData("BulletSO");
    }
}
