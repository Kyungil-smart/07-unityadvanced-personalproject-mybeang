using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    public static Controller Instance;
    private BuildAction _buildAction;
    private Camera _camera;
    private CameraMovement _cameraMovement;
    private Tile _selectedTile;
    // 일단 넣어봐.. 어디서 어떻게 하는게 좋은지 좀.. 생각좀..
    [SerializeField] private MenuUIControl _menuUI;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        _buildAction = new BuildAction();
        _camera = Camera.main;
        _cameraMovement = _camera.gameObject.GetComponent<CameraMovement>();
    }

    private void Update()
    {
        
    }

    private void OnEnable()
    {
        _buildAction.Enable();
        _buildAction.Build.Enable();
        _buildAction.Build.Action.started += OnAction;
        _buildAction.Build.Cancel.started += OnCancel;
        _buildAction.Build.OpenMenu.started += OnOpenMenu;
        _buildAction.Build.GameTime.started += OnGameTime;
        _buildAction.Build.MoveCamera.started += OnMoveCamera;
        _buildAction.Build.TrackingMousePosition.performed += OnTrackingMousePosition;
        
        _buildAction.Build.SelectBuilding.started += OnSelectBuilding;
        _buildAction.Build.RotateBuilding.started += OnRotateBuilding;
        _buildAction.Build.FlipCurvBelt.started += OnFlipCurvBelt;
        _buildAction.Build.SellBuilding.started += OnSellBuilding;
        
        _buildAction.Build.OnTestDamaged.started += OnTestDamaged;
        _buildAction.Build.RunTestScript.started += OnRunTestScript;
    }

    private void OnDisable()
    {
        _buildAction.Disable();
        _buildAction.Build.Action.started -= OnAction;
        _buildAction.Build.Cancel.started -= OnCancel;
        _buildAction.Build.OpenMenu.started -= OnOpenMenu;
        _buildAction.Build.GameTime.started -= OnGameTime;
        _buildAction.Build.MoveCamera.started -= OnMoveCamera;
        _buildAction.Build.TrackingMousePosition.performed -= OnTrackingMousePosition;
        
        _buildAction.Build.SelectBuilding.started -= OnSelectBuilding;
        _buildAction.Build.RotateBuilding.started -= OnRotateBuilding;
        _buildAction.Build.FlipCurvBelt.started -= OnFlipCurvBelt;
        _buildAction.Build.SellBuilding.started -= OnSellBuilding;
        
        _buildAction.Build.OnTestDamaged.started -= OnTestDamaged;
        _buildAction.Build.RunTestScript.started -= OnRunTestScript;
    }

    private void OnRunTestScript(InputAction.CallbackContext obj)
    {
        StartCoroutine(TestScript.Instance.StartScript());
    }

    private void OnSellBuilding(InputAction.CallbackContext obj)
    {
        if (_selectedTile != null) BuildManager.Instance.SellBuilding(_selectedTile);
    }

    private void OnTestDamaged(InputAction.CallbackContext obj)
    {
        PlayerStatusManager.Instance.currentHp -= 50;
    }

    private void OnOpenMenu(InputAction.CallbackContext obj)
    {
        _menuUI.gameObject.SetActive(!_menuUI.gameObject.activeSelf);
    }

    private void OnMoveCamera(InputAction.CallbackContext obj)
    {
        _cameraMovement.IsBattleField = !_cameraMovement.IsBattleField;
        _menuUI.IsBattleField = !_menuUI.IsBattleField;
    }

    private void OnSelectBuilding(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int pressedKey = int.Parse(context.control.name);
            BuildManager.Instance.SelectBuilding(pressedKey, GetMousePosition());
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
                Tile tile;
                if (hit.collider.tag.Contains("Wall"))
                {
                    Wall wall = hit.collider.GetComponent<Wall>();
                    tile = wall.myTile;
                }
                else
                {
                    tile = hit.collider.gameObject.GetComponent<Tile>(); 
                }
                tile.DrawOutline(true);
                _selectedTile = tile;
                if (BuildManager.Instance.SelectedBuilding != null)
                {
                    Debug.Log($"build {BuildManager.Instance.SelectedBuilding.name}");
                    BuildManager.Instance.Build(tile);
                }
                else if (tile.HasObject != null)
                {
                    if (tile.HasObject.GetComponent<FactoryMaster>() != null)
                    {
                        BuildManager.Instance.OpenFactoryUI(tile);
                    }
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
        BuildManager.Instance.CloseFactoryUI();
        BuildManager.Instance.SelectBuilding(-1, Vector2.zero);
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
            // ToDo. 조금 더 생각해보자.
            GameManager.Instance.StateToPause();
        }
    }

    public Vector3 GetMousePosition()
    {
        Vector2 screenPos = _buildAction.Build.TrackingMousePosition.ReadValue<Vector2>();
        return Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
    }
    
    private void OnTrackingMousePosition(InputAction.CallbackContext context)
    {
        if (BuildManager.Instance.SelectedBuilding != null)
        {
            Vector3 position = GetMousePosition();
            if (BuildManager.Instance.SelectedBuilding.tag.Contains("Tower"))
            {
                position = new Vector3(position.x, position.y + 1.0f, position.z);
            }
            BuildManager.Instance.SelectedBuilding.transform.position = position;
        }
            
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (context.started)
            ClearAction();
    }

    private void OnRotateBuilding(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (BuildManager.Instance.SelectedBuilding != null)
            {
                BuildManager.Instance.SelectedBuilding.GetComponent<IRotatable>()?.Rotate();    
            }
            else if (_selectedTile != null)
            {
                _selectedTile.Rotate();
            }
        } 
    }

    private void OnFlipCurvBelt(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (BuildManager.Instance.SelectedBuilding != null)
            {
                BuildManager.Instance.SelectedBuilding.GetComponent<IFlip>()?.Flip();
            } 
            else if (_selectedTile != null)
            {
                _selectedTile.Flip();
            }
        }
    }
    
    public void PickUp(GameObject gameObject)
    {
        // UI 에서 건물 이동시
    }
    
    
}
