using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FactoryUIControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentBulletType;
    [SerializeField] private Button _arrowButton;
    [SerializeField] private Button _bulletButton;
    [SerializeField] private Button _cannonButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _arrowTxt;
    [SerializeField] private TextMeshProUGUI _bulletTxt;
    [SerializeField] private TextMeshProUGUI _cannonTxt;
    
    public FactoryMaster factory;

    private void OnEnable()
    {
        // 현재 타입 가져오기
        _currentBulletType.text = "현재 탄약:  " + 
            (string)DataManager.Instance.uiMessageData.messageData["Bullet"]?[factory.curBulletType.ToString()];
    
        _arrowTxt.text = (string)DataManager.Instance.uiMessageData.messageData["Bullet"]?["Arrow"];
        _bulletTxt.text = (string)DataManager.Instance.uiMessageData.messageData["Bullet"]?["Bullet"];
        _cannonTxt.text = (string)DataManager.Instance.uiMessageData.messageData["Bullet"]?["Cannon"];
        
        _arrowButton.onClick.AddListener(OnSelectArrow);
        _bulletButton.onClick.AddListener(OnSelectBullet);
        _cannonButton.onClick.AddListener(OnSelectCannon);
        _closeButton.onClick.AddListener(OnClose);
    }

    private void OnDisable()
    {
        _arrowButton.onClick.RemoveListener(OnSelectArrow);
        _bulletButton.onClick.RemoveListener(OnSelectBullet);
        _cannonButton.onClick.RemoveListener(OnSelectCannon);
        _closeButton.onClick.RemoveListener(OnClose);
    }

    // 이것을 굳이 MVP 패턴이나 OpserverPattern 으로 변경할 필요가... 있을가..?
    private void OnSelectArrow() => factory.curBulletType = ItemType.Arrow;
    private void OnSelectBullet() => factory.curBulletType = ItemType.Bullet;
    private void OnSelectCannon() => factory.curBulletType = ItemType.Cannon;
    private void OnClose() => gameObject.SetActive(false);
}
