using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour
{
    public ItemType itemType;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void PrintLog(string text) 
        => Debug.Log($"({gameObject.name};{itemType}) {text}");

    public BulletData LoadBulletData(string ScriptableObjectName)
        => DataManager.Instance.bulletData[ScriptableObjectName];
}
