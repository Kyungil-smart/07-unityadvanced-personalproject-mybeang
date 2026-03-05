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
        SceneChangeManager.Instance.Change(1);
    }

    public void OnExitGame()
    {
        Application.Quit();
    }
    
    public void OnOpenHowToPlayPage()
    {
        Application.OpenURL(howToPlayUrl);
    }
}
