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
            SelectedBuilding = BuildingObject.Create(_buildings[selectNumber - 1], position, Quaternion.identity);
    }

    public void Build(Tile tile)
    {
        // 이미 다른 빌딩이 tile 에 있으면 안되야함.
        if (tile.HasObject != null)
        {
            string text = (string)DataManager.Instance.uiMessageData.messageData["WarningWindow"]["AlreayBuildingOnTile"];
            UIControlManager.Instance.WarningText = text;
            return;
        }
        MonoBehaviour script = SelectedBuilding?.GetComponent<MonoBehaviour>();
        // belt type
        if (script != null & script is IBeltBehavior)
        {
            BuildBelt(tile);
        }
        // miner type
        else if (script is Miner)
        {
            BuildMiner(tile);
        }
        // factory type
        else if (script is FactoryMaster)
        {
            BuildFactory(tile);
        }
        // tower
    }

    private void BuildBelt(Tile tile)
    {
        tile.HasObject = SelectedBuilding;
        SelectedBuilding = null;
    }

    private void BuildMiner(Tile tile)
    {
        Miner miner = SelectedBuilding.GetComponent<Miner>();
        if (!miner.SearchMine(tile.GridPos))
        {
            string text = (string)DataManager.Instance.uiMessageData.messageData["WarningWindow"]["MinerMustBuildNearMine"];
            UIControlManager.Instance.WarningText = text;
            return;  
        }
        tile.HasObject = SelectedBuilding;
        SelectedBuilding = null;
    }

    private void BuildFactory(Tile tile)
    {
        tile.HasObject = SelectedBuilding;
        SelectedBuilding = null;
    }

    private void BuildTower()
    {
        
    }
     
}
