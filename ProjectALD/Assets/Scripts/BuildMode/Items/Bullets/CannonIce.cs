using UnityEngine;

public class CannonIce : Item
{
    public BulletData data;
    private void Start()
    {
        data = LoadBulletData("CannonIceSO");
    }
}
