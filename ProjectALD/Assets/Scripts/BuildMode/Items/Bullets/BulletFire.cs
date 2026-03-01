using UnityEngine;

public class BulletFire : Item
{
    public BulletData data;
    private void Start()
    {
        data = LoadBulletData("BulletFireSO");
    }
}
