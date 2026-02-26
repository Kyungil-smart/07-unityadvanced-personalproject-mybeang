using UnityEngine;
using UnityEngine.InputSystem;

public class Tile : MonoBehaviour
{
    public GameObject HasObject;
    public Material _materialOn;
    public Material _materialOff;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    
    public void DrawOutline(bool enable)
    {
        if (enable)
            _spriteRenderer.material = _materialOn;
        else
            _spriteRenderer.material = _materialOff;
    }
    
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = new Color(0f,0f,1f,.5f);
    //     Gizmos.DrawCube(transform.position, Vector3.one * 0.95f);
    // }
}
