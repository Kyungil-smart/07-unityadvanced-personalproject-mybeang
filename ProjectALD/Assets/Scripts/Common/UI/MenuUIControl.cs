using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIControl : MonoBehaviour
{
    public static MenuUIControl Instance;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _reStartButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Transform _buildModeTf;
    [SerializeField] private Transform _battleModeTf;
    [SerializeField] private Camera _camera;
    private bool _isBattleField;

    public bool IsBattleField
    {
        get { return _isBattleField; }
        set
        {
            _isBattleField = value;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        MenuMovement();
    }

    private void OnEnable()
    {
        _resumeButton.onClick.AddListener(OnResume);
        _reStartButton.onClick.AddListener(OnRestart);
        _quitButton.onClick.AddListener(OnQuit);
    }

    private void OnDisable()
    {
        _resumeButton.onClick.RemoveListener(OnResume);
        _reStartButton.onClick.RemoveListener(OnRestart);
        _quitButton.onClick.RemoveListener(OnQuit);
    }

    private void OnResume()
    {
        gameObject.SetActive(false);
    }

    private void OnRestart()
    {
        SceneChangeManager.Instance.Change(4);
    }

    private void OnQuit()
    {
        GameManager.Instance.StateToGameOver();
    }

    private void MenuMovement()
    {
        Vector3 battlePos = new Vector3(_battleModeTf.position.x, _battleModeTf.position.y, 0);
        Vector3 buildPos = new Vector3(_buildModeTf.position.x, _buildModeTf.position.y, 0);
        transform.position = Vector3.Lerp(
            transform.position, 
            _isBattleField ? battlePos : buildPos, 
            Time.deltaTime * 5f);
    }
}