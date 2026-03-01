using UnityEngine;

public class Cannon : Item
{
    public BulletData data;
    private void Start()
    {
        data = LoadBulletData("CannonSO");
    }
}
