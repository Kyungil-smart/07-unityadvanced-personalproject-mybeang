using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    public GameObject TilePrefab;
    public GameObject WallPrefab;
    public List<GameObject> MineList;

    private int _width = 39;
    private int _height = 20;

    private int _startPosX;
    private int _startPosY;
    
    private GameObject[,] _grid;
    
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
        CreateGrid();
        PositioningMines();
    }
    
    private void CreateGrid()
    {
        _grid = new GameObject[_height, _width];
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Vector3 pos = new Vector3(x + _startPosX, y + _startPosY, 0);
                GameObject tile = Instantiate(TilePrefab, pos, Quaternion.identity);
                if (x == _width - 1)
                {
                    Tile t = tile.GetComponent<Tile>();
                    t.HasObject = Instantiate(WallPrefab, pos, Quaternion.identity);
                }
                _grid[y, x] = tile;

            }
        }
    }

    private void PositioningMines()
    {
        int y = 7;
        foreach (var mine in MineList)
        {
            mine.transform.position = new Vector3(-19, y, 0);
            _grid[0, y + 10].GetComponent<Tile>().HasObject = mine;
            y -= 5;
        }
    }
}
