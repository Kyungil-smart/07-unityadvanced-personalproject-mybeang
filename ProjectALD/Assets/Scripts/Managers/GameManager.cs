using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : SingleTon<GameManager>
{
    public int CurrentWave = 1;
    public int TotalWave = 15;

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
    
    private void Awake()
    {
        SingleTonInit();
    }

    private void Start()
    {
        StateToEnterGame(); // 추후 Title 에서 GameStart 버튼 클릭시 작용
    }

    private async Task OnChangeGameState(GameState value)
    {
        // state machine
        switch (value)
        {
            case GameState.EnterGame:
                Init();
                break;
            case GameState.DataLoading:
                await Loading();
                break;
            case GameState.WaveStart:
                StartCoroutine(GameTimeCoroutine());
                break;
            case GameState.Pause:
                Pause();
                break;
            default:
                Resume();
                break;
        }
    }
    
    public void StateToIdle() => gameState = GameState.Idle;
    public void StateToPause() => gameState = GameState.Pause;
    public void StateToDataLoading() => gameState = GameState.DataLoading;
    public void StateToEnterGame() => gameState = GameState.EnterGame;
    public void StateToWaveStart() => gameState = GameState.WaveStart;

    private void Init()
    {
        gameState = GameState.Idle;
        CurrentWave = 1;
        TotalWave = 15;
        PlayerStatusManager.Instance.Init();
        StateToDataLoading();
    }
    
    private IEnumerator GameTimeCoroutine()
    {
        while (CurrentWave <= TotalWave)
        {  
            yield return new WaitForSeconds(300f);
            CurrentWave++;
        }
    }

    private void Pause() => Time.timeScale = 0;
    private void Resume() => Time.timeScale = 1;

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
        StateToIdle();
    }
}
