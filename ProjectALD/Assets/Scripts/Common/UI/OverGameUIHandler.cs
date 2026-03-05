using UnityEngine;

public class OverGameUIHandler : MonoBehaviour
{
    private void OnEnable()
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
