using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatusManager : MonoBehaviour, IInitializable, IDamagable
{
    public static PlayerStatusManager Instance;
    
    // About HP
    public int curHpLevel;
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
    
    public int RepairCost => totalHp;  
    public float HealPoint => totalHp * 0.5f;
    public int curUpHpCost;
    
    // About Gold
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
    
    // About Damage Multiplier
    public int curDmgLevel;
    public float damageMultiplier;
    public int curUpDmgCost;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void UpgardeHp()
    {
        TowerData data = DataManager.Instance.towerData["TowerSO"];
        if (curHpLevel < data.maxUpgradeLevel)
        {
            // level 증가
            curHpLevel++;
            // 골드 증가
            curUpHpCost += data.incrementUpgradeCost;
            // 데이터 수정
            currentHp += data.incrementHP;
            totalHp += data.incrementHP;
        }
        WastGold(curUpHpCost);
        if (curHpLevel == data.maxUpgradeLevel) curHpLevel = 99;
    }

    public void UpgradeDamageMultipler()
    {
        TowerData data = DataManager.Instance.towerData["TowerSO"];
        if (curDmgLevel < data.maxDamageUpgradeLevel)
        {
            // level 증가
            curDmgLevel++;
            // 골드 증가
            curUpDmgCost += data.incrementDamageUpgradeCost;
            // 데이터 수정
            damageMultiplier += data.multiplierDamage;    
        }
        WastGold(curUpDmgCost);
        if (curDmgLevel == data.maxDamageUpgradeLevel) curDmgLevel = 99;
    }

    public void WastGold(int gold)
    {
        Gold -= gold;
    }
    public void EarnGold(int gold)
    {
        Gold += gold;
    }

    public void Init()
    {
        TowerData data = DataManager.Instance.towerData["TowerSO"];
        curHpLevel = 1;
        totalHp = 1000;
        currentHp = totalHp;
        Gold = 1000000; // 테스트용 초기 자금
        // Gold = 3000; // 실제 게임 초기 자금
        curDmgLevel = 1;
        damageMultiplier = 1;
        curUpHpCost = data.initUpgradeCost;
        curUpDmgCost = data.initDamageUpgradeCost;
    }

    public void Repair()
    {
        Gold -= RepairCost;
        currentHp += (int)HealPoint;
        if (currentHp > totalHp) currentHp = totalHp;
    }

    public Task InitDataAsync()
    {
        Init();
        return Task.CompletedTask;
    }
    
    public void TakeDamage(float damage, DamageType damageType)
    {
        currentHp -= (int)damage;
        if (currentHp <= 0) GameManager.Instance.StateToGameOver();
    }
}