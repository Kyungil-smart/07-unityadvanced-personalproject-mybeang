using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;
    public int _selectNumber = -1;
    public UnityEvent OnSelectNumber;
    public int SelectNumber
    {
        get => _selectNumber;
        set
        {
            _selectNumber = value;
            OnSelectNumber?.Invoke();
        }
    }

    public GameObject SelectedBuilding;
    
    [SerializeField] private List<GameObject> _buildings;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnEnable()
    {
        OnSelectNumber.AddListener(SelectBuilding);
    }

    private void OnDisable()
    {
        OnSelectNumber.RemoveListener(SelectBuilding);
    }

    private void SelectBuilding()
    {   // ToDo: Object Pool for buildings
        if (SelectedBuilding != null)
        {
            Destroy(SelectedBuilding);
            SelectedBuilding = null;
        }
        if (_selectNumber > -1)
            SelectedBuilding = Instantiate(_buildings[_selectNumber]);
    }

    public void Build()
    {
        // 이미 다른 빌딩이 tile 에 있으면 안되야함.
        
        // belt type
        // miner type
        // factory type
        // tower
    }

    private void BuildBelt()
    {
        
    }

    private void BuildMiner()
    {
        
    }

    private void BuildFactory()
    {
        
    }

    private void BuildTower()
    {
        
    }
     
}
