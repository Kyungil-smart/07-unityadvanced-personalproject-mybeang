using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using UnityEngine;

public class GridManager : MonoBehaviour, IInitializable
{
    public static GridManager Instance { get; private set; }
    public GameObject TilePrefab;
    public GameObject WallPrefab;
    public List<GameObject> MineList;
    public float WallPosX; 

    private int _width = 25;
    private int _height = 11;

    private int _startPosX;
    private int _startPosY;
    
    private Tile[,] _grid;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _startPosX = (int)transform.position.x;
        _startPosY = (int)transform.position.y;
    }
    
    private void CreateGrid()
    {
        _grid = new Tile[_height, _width];
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Vector3 pos = new Vector3(x + _startPosX, y + _startPosY, 0);
                GameObject tile = Instantiate(TilePrefab, pos, Quaternion.identity);
                Tile t = tile.GetComponent<Tile>();
                t.GridPos = new Vector2Int(x, y);
                if (x == _width - 1)
                {
                    t.HasObject = Instantiate(WallPrefab, pos, Quaternion.identity);
                    WallPosX = t.transform.position.x;
                }
                _grid[y, x] = t;
            }
        }
    }

    private void PositioningMines()
    {
        int y = 7;
        foreach (var mine in MineList)
        {
            mine.transform.position = _grid[y, 0].transform.position;
            _grid[y, 0].HasObject = mine;
            y -= 4;
        }
    }

    public GameObject GetObjectOnTile(Vector2Int pos)
    {
        return GetObjectOnTile(pos.x, pos.y);
    }
    public GameObject GetObjectOnTile(int x, int y)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height) return null;
        return _grid[y, x].HasObject;
    }

    public GameObject[] GetObjectsAroundTile(Vector2Int pos)
    {
        return GetObjectsAroundTile(pos.x, pos.y);
    }

    public GameObject[] GetObjectsAroundTile(int x, int y)
    {
        List<GameObject> objects = new List<GameObject>();
        (int y, int x)[] directions = {(0, 1), (0, -1), (1, 0), (-1, 0)};
        foreach (var dir in directions)
        {
            if (dir.x + x < 0 || dir.x + x >= _width) continue;
            if (dir.y + y < 0 || dir.y + y >= _height) continue;
            GameObject go = _grid[dir.y + y, dir.x + x]?.HasObject; 
            if (go) objects.Add(go);
        }
        return objects.ToArray();
    }

    public bool IsTargetObjectOnAroundTile(Vector2Int pos, GameObject targetObject)
    {
        return IsTargetObjectOnAroundTile(pos.x, pos.y, targetObject);
    }
    
    public bool IsTargetObjectOnAroundTile(int x, int y, GameObject targetObject)
    {
        (int y, int x)[] directions = {(0, 1), (0, -1), (1, 0), (-1, 0)};
        foreach (var dir in directions)
        {
            if (_grid[dir.y + y, dir.x + x].HasObject == targetObject)
            {
                return true;
            }
        }
        return false;
    }

    public Task InitDataAsync()
    {
        CreateGrid();
        PositioningMines();
        return Task.CompletedTask;
    }
}
