using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUIControl : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private Button hpBtn;
    [SerializeField] private TextMeshProUGUI hpLevelTxt;
    [SerializeField] private TextMeshProUGUI hpCostTxt;
    
    [Header("Damage")]
    [SerializeField] private Button dmgBtn;
    [SerializeField] private TextMeshProUGUI dmgLevelTxt;
    [SerializeField] private TextMeshProUGUI dmgCostTxt;
    
    [Header("Others")]
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button ironUnlockBtn;

    private string _lvTxtFmt = "레벨: ";
    private string _goldTxtFmt = "골드: ";
    private string _lvMaxFmt = "레벨: 최대";

    private void OnEnable()
    {
        hpBtn.onClick.AddListener(OnUpgradeHp);
        dmgBtn.onClick.AddListener(OnUpgradeDamageMultiplier);
        closeBtn.onClick.AddListener(CloseWindow);   
        ironUnlockBtn.onClick.AddListener(OnUnlockIronMine);
    }

    private void OnDisable()
    {
        hpBtn.onClick.RemoveListener(OnUpgradeHp);
        dmgBtn.onClick.RemoveListener(OnUpgradeDamageMultiplier);
        closeBtn.onClick.RemoveListener(CloseWindow);
        ironUnlockBtn.onClick.RemoveListener(OnUnlockIronMine);
    }

    private void OnUnlockIronMine()
    {
        AudioManager.Instance.PlayClickSound();
        BuildManager.Instance.UnlockMine();
        ironUnlockBtn.interactable = false;
    }

    private void OnUpgradeHp()
    {
        PlayerStatusManager.Instance.UpgardeHp();
        if (PlayerStatusManager.Instance.curHpLevel == 99)
        {
            hpLevelTxt.text = $"{_lvMaxFmt}";
            hpCostTxt.text = $"{_goldTxtFmt}-";
            hpBtn.interactable = false;
        }
        else
        {
            hpLevelTxt.text = $"{_lvTxtFmt}{PlayerStatusManager.Instance.curHpLevel}";
            hpCostTxt.text = $"{_goldTxtFmt}{PlayerStatusManager.Instance.curUpHpCost}";
            AudioManager.Instance.PlayClickSound();
        }
    }
    
    private void OnUpgradeDamageMultiplier()
    {
        PlayerStatusManager.Instance.UpgradeDamageMultipler();
        if (PlayerStatusManager.Instance.curDmgLevel == 99)
        {
            dmgLevelTxt.text = $"{_lvMaxFmt}";
            dmgCostTxt.text = $"{_goldTxtFmt}-";
            dmgBtn.interactable = false;
        }
        else
        {
            dmgLevelTxt.text = $"{_lvTxtFmt}{PlayerStatusManager.Instance.curDmgLevel}";
            dmgCostTxt.text = $"{_goldTxtFmt}{PlayerStatusManager.Instance.curUpDmgCost}";
            AudioManager.Instance.PlayClickSound();
        }
    }
    
    private void CloseWindow()
    {
        AudioManager.Instance.PlayClickSound();
        gameObject.SetActive(false);
    }
}
