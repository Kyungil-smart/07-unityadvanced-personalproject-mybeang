using UnityEngine;

public class IconDarkMater : Item
{
    public BulletData data;
    private void Start()
    {
        data = LoadBulletData("DarkMatterSO");
    }
}
