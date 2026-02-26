using UnityEngine;
using UnityEngine.UI;

public abstract class ObjectOnTile : MonoBehaviour
{
    public Tile myTile;
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    
    public void SetLayerPriority(int priority)
    {
        if (_spriteRenderer != null) 
            _spriteRenderer.sortingOrder = priority;
    }
}
