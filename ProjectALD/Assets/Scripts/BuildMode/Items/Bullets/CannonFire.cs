using UnityEngine;

public class CannonFire : Item
{
    public BulletData data;
    private void Start()
    {
        data = LoadBulletData("CannonFireSO");
    }
}
