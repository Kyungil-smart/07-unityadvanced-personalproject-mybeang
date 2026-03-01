using UnityEngine;

public class BulletIce : Item
{
    public BulletData data;
    private void Start()
    {
        data = LoadBulletData("BulletIceSO");
    }
}
