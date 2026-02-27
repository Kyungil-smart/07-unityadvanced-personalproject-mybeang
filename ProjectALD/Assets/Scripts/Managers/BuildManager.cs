using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;
    public GameObject SelectedBuilding;
    
    [SerializeField] private List<GameObject> _buildings;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void SelectBuilding(int selectNumber, Vector3 position)
    {   // ToDo: Object Pool for buildings
        if (SelectedBuilding != null)
        {
            Destroy(SelectedBuilding);
            SelectedBuilding = null;
        }
        if (selectNumber > -1)
            SelectedBuilding = Instantiate(_buildings[selectNumber - 1], position, Quaternion.identity);
    }

    public void Build(Tile tile)
    {
        // 이미 다른 빌딩이 tile 에 있으면 안되야함.
        if (tile.HasObject != null)
        {
            UIControlManager.Instance.ShowDoNotbuildWarning();
            return;
        }
        // belt type
        // miner type
        if (SelectedBuilding.GetComponent<Miner>() != null)
        {
            BuildMiner(tile);
        }
        // factory type
        // tower
    }

    private void BuildBelt()
    {
        
    }

    private void BuildMiner(Tile tile)
    {
        tile.HasObject = SelectedBuilding;
        SelectedBuilding = null;
    }

    private void BuildFactory()
    {
        
    }

    private void BuildTower()
    {
        
    }
     
}
