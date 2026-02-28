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
    
    public int HP;
    public int RepairCost => HP;
    public float HealPoint => HP * 0.5f;
    
    private int _gold = 1000000;
    public int Gold { 
        get => _gold;
        set
        {
            _gold = value;
            OnChangedGold?.Invoke();
        }
    }
    public UnityEvent OnChangedGold;
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
