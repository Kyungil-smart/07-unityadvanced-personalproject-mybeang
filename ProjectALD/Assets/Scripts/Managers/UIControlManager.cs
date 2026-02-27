using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIControlManager : MonoBehaviour
{
    public static UIControlManager Instance;
    [SerializeField] private GameObject _doNotbuildWarning;

    public void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    public void ShowDoNotbuildWarning()
    {
        StartCoroutine(DoNotbuildWarningCoroutine());
    }

    private IEnumerator DoNotbuildWarningCoroutine()
    {
        _doNotbuildWarning.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        _doNotbuildWarning.SetActive(false);
    }
}
