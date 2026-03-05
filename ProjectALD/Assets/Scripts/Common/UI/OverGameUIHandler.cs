using UnityEngine;

public class OverGameUIHandler : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.StopBgm();
    }
    
    public void OnToTitle()
    {
        AudioManager.Instance.PlayClickSound();
        SceneChangeManager.Instance.Change(0);
    }

    public void OnExitGame()
    {
        AudioManager.Instance.PlayClickSound();
        Application.Quit();
    }
}
