using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;
    public GameObject SelectedBuilding;
    private int _selectedNumber;
    
    [SerializeField] private List<GameObject> _buildings;
    [SerializeField] private FactoryUIControl _factoryUI;
    [SerializeField] private Mine _copperMine;
    [SerializeField] private Mine _ironMine;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void OpenBuildWarningMessage(string key)
    {
        string text = (string)DataManager.Instance.uiMessageData.messageData["WarningWindow"][key];
        MainUIControl.Instance.WarningText = text;
    }
    
    public void SelectBuilding(int selectNumber, Vector3 position)
    {   // ToDo: Object Pool for buildings
        _selectedNumber = selectNumber;
        if (SelectedBuilding != null)
        {
            Destroy(SelectedBuilding);
            SelectedBuilding = null;
        }

        if (selectNumber > -1)
        {
            if (selectNumber >= 7 && selectNumber <= 9)
            {
                OpenBuildWarningMessage("ToDoBuilding");
                return;
            }
            if (selectNumber == 0) selectNumber = _buildings.Count - 1;
            else selectNumber -= 1;
            SelectedBuilding = BuildingObject.Create(_buildings[selectNumber], position, Quaternion.identity);
        }
    }

    private bool AlreayOnBuild(Tile tile)
    {
        if (tile.HasObject != null)
        {
            Wall wall = tile.HasObject.GetComponent<Wall>();
            if (wall != null && wall.DefenceUnit == null && SelectedBuilding.tag.Contains("Tower"))
                return true;
            return false;
        }
        return true;
    }
    
    public void Build(Tile tile)
    {
        // 이미 다른 빌딩이 tile 에 있으면 안되야함.
        if (!AlreayOnBuild(tile))
        {
            OpenBuildWarningMessage("AlreayBuildingOnTile");
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
        else if (script is Tower)
        {
            BuildTower(tile);
        }
    }

    private void BuildBelt(Tile tile)
    {
        tile.HasObject = SelectedBuilding;
        if (_selectedNumber == 1)
        {
            Vector3 position = SelectedBuilding.transform.position;
            Quaternion rotation = SelectedBuilding.transform.rotation;
            SelectedBuilding = BuildingObject.Create(_buildings[_selectedNumber-1], position, rotation);
        }
        else
        {
            SelectedBuilding = null;    
        }
    }

    private void BuildMiner(Tile tile)
    {
        Miner miner = SelectedBuilding.GetComponent<Miner>();
        if (!miner.SearchMine(tile.GridPos))
        {
            OpenBuildWarningMessage("MinerMustBuildNearMine");
            return;  
        }
        tile.HasObject = SelectedBuilding;
        PlayerStatusManager.Instance.WastGold(DataManager.Instance.buildCostData["Miner"]);
        SelectedBuilding = null;
    }

    private void BuildFactory(Tile tile)
    {
        tile.HasObject = SelectedBuilding;
        OpenFactoryUI(tile);
        PlayerStatusManager.Instance.WastGold(DataManager.Instance.buildCostData["Factory"]);
        SelectedBuilding = null;
    }

    public void OpenFactoryUI(Tile tile)
    {
        FactoryMaster factory = tile.HasObject.GetComponent<FactoryMaster>();
        if (factory != null)
        {
            _factoryUI.factory = factory;
            _factoryUI.gameObject.SetActive(true);
        }
    }

    public void CloseFactoryUI()
    {
        _factoryUI.gameObject.SetActive(false);
    }

    private void BuildTower(Tile tile)
    {
        if (tile.HasObject?.GetComponent<Wall>() == null)
        {
            OpenBuildWarningMessage("TowerShoudBeOnWall");
            return;
        }
        Wall wall = tile.HasObject.GetComponent<Wall>();
        wall.DefenceUnit = SelectedBuilding;
        Vector3 curPos = wall.DefenceUnit.transform.position;
        Vector3 newPos = new Vector3(curPos.x, curPos.y + 1f, 0);
        wall.DefenceUnit.transform.position = newPos;
        PlayerStatusManager.Instance.WastGold(DataManager.Instance.buildCostData["Tower"]);
        SelectedBuilding = null;
    }

    public void UnlockMine()
    {
        // 추후 여러 mine 에 대해 선택 할 수 있는 logic 필요.
        // 현 시점 ironMine 만 해금하므로 해당 코드만 진행.
        _ironMine.Unlock();
    }

    public void SellBuilding(Tile tile)
    {
        if (tile.HasObject == null) return;
        tile.SellBuilding();
    }
}
