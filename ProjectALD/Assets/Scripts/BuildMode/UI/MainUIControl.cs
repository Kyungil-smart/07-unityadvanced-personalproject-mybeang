using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class MainUIControl : MonoBehaviour, IInitializable
{
    public static MainUIControl Instance;
    // UI 제어용 Inspector 에 노출하여 연결해 주어야 할 것들.
    [Header("For Warning Text")]
    [SerializeField] private GameObject _WarningWindow;
    [SerializeField] private TextMeshProUGUI _warningText;
    
    [Header("For Global Game State")]
    [SerializeField] private Image _currentHpImage;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private Button _waveStartButton;
    
    [Header("For UI Control")]
    [SerializeField] private Image _buildIconBar;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _repairButton;
    [SerializeField] private Canvas _upgardeUICanvas;
    
    [Header("For Build Button")]
    [SerializeField] private List<Button> _buildButtons;
    [SerializeField] private List<TextMeshProUGUI> _buildButtonCost;
    
    // 그외 Inspector 에서는 신경쓰지 않아도 될 것들.
    public UnityEvent<string> OnChangedWarningText;
    public string WarningText
    {
        get
        {
            return _warningText.text;
        }
        set
        {
            _warningText.text = value;
            OnChangedWarningText?.Invoke(value);
        }
    }
    private CameraMovement _mainCameraMovement;

    public void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void Start()
    {
        _mainCameraMovement = Camera.main?.GetComponent<CameraMovement>();
    }

    public void Update()
    {
        MoveFieldHandler();
    }

    public void OnEnable()
    {
        OnChangedWarningText.AddListener(OpenWarning);
        PlayerStatusManager.Instance.OnChangeCurrentHp.AddListener(ControlHpGage);
        PlayerStatusManager.Instance.OnChangeCurrentHp.AddListener(ChangeCurrentHp);
        PlayerStatusManager.Instance.OnChangeTotalHp.AddListener(ChangeTotalHp);
        PlayerStatusManager.Instance.OnChangedGold.AddListener(ChangeGold);
        
        // add handler to buttons
        _upgradeButton.onClick.AddListener(OnUpgradeWindowOpen);
        _repairButton.onClick.AddListener(OnRepair);
        for (int i = 0; i < _buildButtons.Count; i++)
        {
            int index = i + 1;
            _buildButtons[i].onClick.AddListener(() => OnClickBuildButton(index));
        }
        _waveStartButton?.onClick.AddListener(OnWaveStart);
    }

    public void OnDisable()
    {
        OnChangedWarningText.RemoveListener(OpenWarning);
        PlayerStatusManager.Instance.OnChangeCurrentHp.RemoveListener(ControlHpGage);
        PlayerStatusManager.Instance.OnChangeCurrentHp.RemoveListener(ChangeCurrentHp);
        PlayerStatusManager.Instance.OnChangeTotalHp.RemoveListener(ChangeTotalHp);
        PlayerStatusManager.Instance.OnChangedGold.RemoveListener(ChangeGold);
        
        // remove handler to buttons
        _upgradeButton.onClick.RemoveListener(OnUpgradeWindowOpen);
        _repairButton.onClick.RemoveListener(OnRepair);
        foreach (Button button in _buildButtons)
            button.onClick.RemoveAllListeners();
        
        _waveStartButton?.onClick.RemoveListener(OnWaveStart);
    }

    private void InitBuildCost()
    {
        foreach (var costTxt in _buildButtonCost)
        {
            string name = costTxt.name.Replace("Cost", string.Empty);
            if (DataManager.Instance.buildCostData.ContainsKey(name))
            {
                costTxt.text = DataManager.Instance.buildCostData[name].ToString();
            }
        }
    }

    public void OpenWarning(string text)
    {
        StartCoroutine(WarningCoroutine(text));
    }

    private IEnumerator WarningCoroutine(string text)
    {
        _warningText.text = text;
        _WarningWindow.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        _WarningWindow.SetActive(false);
    }

    public void MoveFieldHandler()
    {
        if (_mainCameraMovement != null)
        {
            _buildIconBar.gameObject.SetActive(!_mainCameraMovement.IsBattleField);
            _upgradeButton.gameObject.SetActive(!_mainCameraMovement.IsBattleField);    
        }
    }
    
    private void ControlHpGage(int currentHp) =>
        _currentHpImage.fillAmount = (float)currentHp / PlayerStatusManager.Instance.totalHp;
    
    private void ChangeCurrentHp(int currentHp) =>
        _hpText.text = $"{currentHp} / {PlayerStatusManager.Instance.totalHp}";
    
    private void ChangeTotalHp(int totalHp) =>
        _hpText.text = $"{PlayerStatusManager.Instance.currentHp} / {totalHp}";
    
    private void ChangeGold(int gold) =>
        _goldText.text = gold.ToString();
    
    public Task InitDataAsync()
    {
        InitBuildCost();
        return Task.CompletedTask;
    }
    
    // === buttonhandler =========
    private void OnWaveStart()
    {
        GameManager.Instance.StateToWaveStart();
        _waveStartButton.gameObject.SetActive(false);
    }

    private void OnUpgradeWindowOpen()
    {
        _upgardeUICanvas.gameObject.SetActive(!_upgardeUICanvas.gameObject.activeSelf);
    }

    private void OnClickBuildButton(int index)
    {
        BuildManager.Instance.SelectBuilding(index, Controller.Instance.GetMousePosition());
    }

    private void OnRepair()
    {
        PlayerStatusManager.Instance.Repair();
    }
}
