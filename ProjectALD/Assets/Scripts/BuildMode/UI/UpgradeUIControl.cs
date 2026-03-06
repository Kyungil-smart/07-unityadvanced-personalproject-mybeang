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
    
    private void OpenBuildWarningMessage(string key)
    {
        string text = (string)DataManager.Instance.uiMessageData.messageData["WarningWindow"][key];
        MainUIControl.Instance.WarningText = text;
    }

    private void OnUnlockIronMine()
    {
        AudioManager.Instance.PlayClickSound();
        int unlockCost = DataManager.Instance.minerData[$"IronMinerSO"].unlockCost;
        if (!PlayerStatusManager.Instance.IsEnoughGold(unlockCost)) return;
        BuildManager.Instance.UnlockMine();
        ironUnlockBtn.interactable = false;
    }

    private void OnUpgradeHp()
    {
        AudioManager.Instance.PlayClickSound();
        if (!PlayerStatusManager.Instance.IsEnoughGold(PlayerStatusManager.Instance.curUpHpCost)) return;
        
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
        }
    }
    
    private void OnUpgradeDamageMultiplier()
    {
        AudioManager.Instance.PlayClickSound();
        if (!PlayerStatusManager.Instance.IsEnoughGold(PlayerStatusManager.Instance.curUpDmgCost)) return;
        
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
        }
    }
    
    private void CloseWindow()
    {
        AudioManager.Instance.PlayClickSound();
        gameObject.SetActive(false);
    }
}
