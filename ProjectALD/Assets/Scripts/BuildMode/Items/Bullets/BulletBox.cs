using System.Collections.Generic;
using UnityEngine;

public class BulletBox : Item
{
    public BulletData data;
    public ItemType label;
    private Queue<GameObject> items;
    public SpriteRenderer bulletSpriteRenderer;
    private void Start()
    {
        items = new Queue<GameObject>();
    }

    public void SetLabel(ItemType itemType, SpriteRenderer spriteRenderer)
    {
        label = itemType;
        PrintLog($"load data name: {itemType}SO");
        data = LoadBulletData($"{itemType}SO");
        if (data == null)
            PrintLog($"Fail to load data; {itemType}SO");
        bulletSpriteRenderer = spriteRenderer;
    }

    public bool IsFull()
    {
        if (items == null) items = new Queue<GameObject>();
        PrintLog($"current items count: {items.Count}");
        PrintLog($"Max count: {data.count}");
        return items.Count == data.count;
    } 

    public void PutBullet(GameObject bulletObj)
    {   // 공장에서 쓸 용도
        if (items.Count < data.count) 
            items.Enqueue(bulletObj);
    }
    
    public GameObject GetBullet(GameObject bulletObj)
    {   // 타워에서 쓸 용도
        if (items.Count > 0)
            return items.Dequeue();
        return null;
    }
}
