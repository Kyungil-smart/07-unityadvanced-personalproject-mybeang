using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{  
    private BuildAction _buildAction;
    private Camera _camera;
    private Tile _selectedTile;
    
    private void Awake()
    {
        _buildAction = new BuildAction();
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _buildAction.Enable();
        _buildAction.Build.Enable();
        _buildAction.Build.SelectBuilding.started += OnSelectBuilding;
        _buildAction.Build.Action.started += OnAction;
        _buildAction.Build.GameTime.started += OnGameTime;
        _buildAction.Build.TrackingMousePosition.performed += OnTrackingMousePosition;
        _buildAction.Build.Cancel.started += OnCancel;
    }

    private void OnDisable()
    {
        _buildAction.Disable();
        _buildAction.Build.SelectBuilding.started -= OnSelectBuilding;
        _buildAction.Build.Action.started -= OnAction;
        _buildAction.Build.GameTime.started -= OnGameTime;
        _buildAction.Build.TrackingMousePosition.performed -= OnTrackingMousePosition;
        _buildAction.Build.Cancel.started -= OnCancel;
    }

    private void OnSelectBuilding(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int pressedKey = int.Parse(context.control.name);
            BuildManager.Instance.SelectNumber = pressedKey;
        }
    }
    
    private void OnAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_selectedTile != null) _selectedTile.DrawOutline(false);
            if (IsPointerOverUI()) return;
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {
                Tile tile = hit.collider.gameObject.GetComponent<Tile>();
                tile.DrawOutline(true);
                _selectedTile = tile;
                if (BuildManager.Instance.SelectedBuilding != null)
                {
                    Debug.Log($"{BuildManager.Instance.SelectedBuilding.name} 빌딩을 지어라");
                } 
                else if (tile.HasObject != null)
                {
                    Debug.Log("해당하는 메뉴를 열어라");
                }
            }
            else
            {
                ClearAction();
            }
        }
    }

    private void ClearAction()
    {
        if (_selectedTile != null) _selectedTile.DrawOutline(false);
        _selectedTile = null;
        BuildManager.Instance.SelectNumber = -1;
    }
    
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
    
    private void OnGameTime(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.IsPause = !GameManager.Instance.IsPause;    
        }
    }

    private void OnTrackingMousePosition(InputAction.CallbackContext context)
    {
        Vector2 screenPos = _buildAction.Build.TrackingMousePosition.ReadValue<Vector2>();
        if (BuildManager.Instance.SelectedBuilding != null)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
            BuildManager.Instance.SelectedBuilding.transform.position = worldPos;
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (context.started)
            ClearAction();
    }
}
