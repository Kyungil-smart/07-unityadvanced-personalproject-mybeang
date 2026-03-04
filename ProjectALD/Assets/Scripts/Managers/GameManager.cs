using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : SingleTon<GameManager>
{
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
                Debug.Log("Enter Game");
                Init();
                break;
            case GameState.DataLoading:
                Debug.Log("Data Loading");
                await Loading();
                break;
            case GameState.WaveStart:
                StartCoroutine(GameTimeCoroutine());
                break;
            case GameState.Pause:
                Debug.Log("Pause Game");
                Pause();
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
            yield return new WaitForSecondsRealtime(30f);  // wave test 후 300f 로 원복하기
            CurrentWave++;
        }
    }

    private void Pause() => Time.timeScale = 0;
    private void Resume() => Time.timeScale = 1;

    private async Task Loading()
    {
        string name = "";
        LoadingTotalStep = InitialTargetObjects.Count;
        foreach (var i in InitialTargetObjects)
        {
            LoadingCurStep++;
            if (i is IInitializable initializable)
            {
                name = initializable.ToString();
                await initializable.InitDataAsync();
            }
            Debug.Log($"Loading - {name} {LoadingCurStep}/{LoadingTotalStep}");
        }
        StateToIdle();
    }
}
