using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : SingleTon<GameManager>
{
    public bool IsPause;
    public int CurrentWave = 1;
    public int TotalWave = 30;

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
    public int maximumHp = 3000;
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
    public List<MonoBehaviour> InitialTargetObjects;
    private int _loadingCurStep = 0;
    public UnityEvent OnChangedCurrentStep;
    public int LoadingCurStep
    {
        get => _loadingCurStep;
        set
        {
            _loadingCurStep = value;
            OnChangedCurrentStep?.Invoke();
        }
    }
    public int LoadingTotalStep;
    
    private void Awake()
    {
        SingleTonInit();
    }

    private async void Start()
    {
        totalHp = 100;
        currentHp = totalHp;
        Gold = 1000000;
        await Loading();
        StartCoroutine(GameTimeCoroutine());
    }
    
    private IEnumerator GameTimeCoroutine()
    {
        while (CurrentWave <= TotalWave)
        {  
            yield return new WaitForSeconds(300f);
            CurrentWave++;
        }
    }

    private async Task Loading()
    {
        LoadingTotalStep = InitialTargetObjects.Count;
        foreach (var i in InitialTargetObjects)
        {
            LoadingCurStep++;
            if (i is IInitializable initializable)
            {
                await initializable.InitDataAsync();
            }
            Debug.Log($"Loading {LoadingCurStep}/{LoadingTotalStep}");
        }
    }
}
