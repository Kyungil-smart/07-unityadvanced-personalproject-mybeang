using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : SingleTon<GameManager>
{
    public bool IsPause;
    public int CurrentWave = 1;
    public int TotalWave = 30;
    private int _gold;
    public int Gold { 
        get => _gold;
        set
        {
            _gold = value;
            OnChangedGold?.Invoke();
        }
    }
    public UnityEvent OnChangedGold;
    
    private void Awake()
    {
        SingleTonInit();
    }

    private void Start()
    {
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
}
