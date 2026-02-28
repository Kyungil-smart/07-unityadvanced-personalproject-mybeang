using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class UIControlManager : MonoBehaviour
{
    public static UIControlManager Instance;
    [SerializeField] private GameObject _WarningWindow;
    [SerializeField] private TextMeshProUGUI _warningText;
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

    public void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void OnEnable()
    {
        OnChangedWarningText.AddListener(OpenWarning);
    }

    public void OnDisable()
    {
        OnChangedWarningText.RemoveListener(OpenWarning);
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
}
