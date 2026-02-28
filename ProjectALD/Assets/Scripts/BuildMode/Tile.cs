using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Tile : MonoBehaviour, IPlacable, IRotatable
{
    private GameObject _hasObject;

    public GameObject HasObject
    {
        get { return _hasObject; }
        set
        {
            _hasObject = value;
            OnChangedObject?.Invoke();
        }
    }
    public UnityEvent OnChangedObject;
    public Material _materialOn;
    public Material _materialOff;
    private SpriteRenderer _spriteRenderer;
    public Vector2Int GridPos;

    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        DrawOutline(false);
    }

    private void OnEnable()
    {
        OnChangedObject.AddListener(PutOnObject);
    }

    private void OnDisable()
    {
        OnChangedObject.RemoveListener(PutOnObject);
    }

    public void DrawOutline(bool enable)
    {
        if (enable)
            _spriteRenderer.material = _materialOn;
        else
            _spriteRenderer.material = _materialOff;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f,0f,1f,.5f);
        Gizmos.DrawCube(transform.position, Vector3.one * 0.95f);
    }

    private void PutOnObject()
    {
        int priority = 100 - GridPos.y;
        ObjectOnTile objectOnTile = HasObject.GetComponent<ObjectOnTile>();
        HasObject.transform.position = transform.position;
        objectOnTile.myTile = this;
        objectOnTile.SetLayerPriority(priority);
        objectOnTile.PutOnTileHandler();
    }

    public void Rotate()
    {
        (HasObject.GetComponent<MonoBehaviour>() as IRotatable)?.Rotate();
    }
}
