using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int _currentWave;
    public UnityEvent<int> OnChangeCurrentWave;
    public int CurrentWave
    {
        get => _currentWave;
        private set
        {
            _currentWave = value;
            OnChangeCurrentWave?.Invoke(value);
        }
    }
    public int TotalWave;

    private GameState _gameState;
    public GameState gameState
    {
        get { return _gameState; }
        set
        {
            _gameState = value;
            OnChangeGameState(value);
        }
    }

    public List<MonoBehaviour> InitialTargetObjects;
    private int _loadingCurStep = 0;
    public UnityEvent OnChangedCurrentStep;
    public int LoadingCurStep
    {
        get => _loadingCurStep;
        private set
        {
            _loadingCurStep = value;
            OnChangedCurrentStep?.Invoke();
        }
    }
    public int LoadingTotalStep;

    private string _dataLoadingUpdateTxt;
    public UnityEvent<string> OnDataLoadingUpdateTxt;

    public string DataLoadingUpdateTxt
    {
        get { return _dataLoadingUpdateTxt; }
        private set
        {
            _dataLoadingUpdateTxt = value;
            OnDataLoadingUpdateTxt?.Invoke(value);
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        StateToEnterGame(); 
    }

    private async Task OnChangeGameState(GameState value)
    {
        // state machine
        switch (value)
        {
            case GameState.EnterGame:
                Debug.Log("Enter Game");
                Init();
                break;
            case GameState.DataLoading:
                AudioManager.Instance.PlayBgm("BgmBuildMode");
                Debug.Log("Data Loading");
                await Loading();
                break;
            case GameState.DataLoadingDone:
                CurtainUp();
                break;
            case GameState.WaveStart:
                StartCoroutine(GameTimeCoroutine());
                break;
            case GameState.Pause:
                Debug.Log("Pause Game");
                Pause();
                break;
            case GameState.GameWon:
                Debug.Log("Game Won");
                ToGameVictory();
                break;
            case GameState.GameOver:
                Debug.Log("Game Over");
                ToGameOver();
                break;
            default:
                Debug.Log("Idle");
                Resume();
                break;
        }
    }
    
    public void StateToIdle() => gameState = GameState.Idle;
    public void StateToPause() => gameState = GameState.Pause;
    public void StateToDataLoading() => gameState = GameState.DataLoading;
    public void StateToEnterGame() => gameState = GameState.EnterGame;

    public void StateToWaveStart() => gameState = GameState.WaveStart;
    public void StateToDataLodingDone() => gameState = GameState.DataLoadingDone;
    public void StateToGameOver() => gameState = GameState.GameOver;
    public void StateToGameVictory() => gameState = GameState.GameWon;

    private void Init()
    {
        gameState = GameState.Idle;
        CurrentWave = 1;
        TotalWave = 15;
        StateToDataLoading();
    }
    
    private IEnumerator GameTimeCoroutine()
    {
        Debug.Log("Wave Start");
        while (CurrentWave <= TotalWave)
        {  
            Debug.Log($"Current wave: {CurrentWave}");
            yield return new WaitForSecondsRealtime(120f); 
            CurrentWave++;
        }
        StateToGameVictory();
    }

    private void Pause() => Time.timeScale = 0;
    private void Resume() => Time.timeScale = 1;
    
    private async Task Loading()
    {
        Debug.Log("Loading Start");
        LoadingTotalStep = InitialTargetObjects.Count;
        foreach (var i in InitialTargetObjects)
        {
            LoadingCurStep++;
            if (i is IInitializable initializable)
                await initializable.InitDataAsync();
            
            DataLoadingUpdateTxt = $"데이터 처리: {i.name} {LoadingCurStep}/{LoadingTotalStep}";
            Debug.Log(DataLoadingUpdateTxt);
            await Task.Delay(500);
        }
        StateToDataLodingDone();
    }

    private void CurtainUp()
    {
        MainUIControl.Instance.CurtainUp();
        StateToIdle();
    }

    private void ToGameVictory()
    {
        SceneChangeManager.Instance.Change(2);
    }

    private void ToGameOver()
    {
        SceneChangeManager.Instance.Change(3);
    }
}
