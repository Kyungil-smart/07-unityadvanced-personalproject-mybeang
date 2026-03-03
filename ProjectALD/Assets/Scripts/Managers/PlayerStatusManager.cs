using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatusManager : MonoBehaviour
{
    public static PlayerStatusManager Instance;
    private int _currentHp;
    public int currentHp
    {
        get { return _currentHp; }
        set
        {
            _currentHp = value;
            OnChangeCurrentHp?.Invoke(value);
        }
    }
    public UnityEvent<int> OnChangeCurrentHp;
    private int _totalHp;
    public UnityEvent<int> OnChangeTotalHp;
    public int totalHp
    {
        get { return _totalHp; }
        set
        {
            _totalHp = value;
            OnChangeTotalHp?.Invoke(value);
        }
    }
    public int maximumHp;
    public int RepairCost => totalHp;  // 어디에 두는게 좋을까?
    public float HealPoint => totalHp * 0.5f;  // 어디에 두는게 좋을까?
    
    private int _gold;
    public int Gold { 
        get => _gold;
        set
        {
            _gold = value;
            OnChangedGold?.Invoke(value);
        }
    }
    public UnityEvent<int> OnChangedGold;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void UpgardeHp()
    {
        
    }

    public void Init()
    {
        totalHp = 1000;
        currentHp = totalHp;
        maximumHp = 5000;
        Gold = 1000000;
    }

    public void Repair()
    {
        Gold -= RepairCost;
        currentHp += (int)HealPoint;
    }
}