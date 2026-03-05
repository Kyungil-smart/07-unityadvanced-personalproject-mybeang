using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public string howToPlayUrl;
    private bool IsBgmPlaying;

    private void Update()
    {
        if (!IsBgmPlaying && AudioManager.Instance.loadingCompleted)
        {
            AudioManager.Instance.PlayBgm("BgmTitle");
            IsBgmPlaying = true;
        }
    }

    public void OnStartGame()
    {
        AudioManager.Instance.PlayClickSound();
        SceneChangeManager.Instance.Change(1);
    }

    public void OnExitGame()
    {
        AudioManager.Instance.PlayClickSound();
        Application.Quit();
    }
    
    public void OnOpenHowToPlayPage()
    {
        AudioManager.Instance.PlayClickSound();
        Application.OpenURL(howToPlayUrl);
    }
}
