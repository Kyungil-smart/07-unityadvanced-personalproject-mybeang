using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;
    public int _selectNumber;
    public UnityEvent OnSelectNumber;
    public int SelectNumber
    {
        get => _selectNumber;
        set
        {
            OnSelectNumber?.Invoke();
            _selectNumber = value;
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

    public void SelectBuilding()
    {
        GameObject building = Instantiate(_buildings[_selectNumber]);
    }
     
}
