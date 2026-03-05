using UnityEngine;

public class OverGameUIHandler : MonoBehaviour
{
    public void OnToTitle()
    {
        SceneChangeManager.Instance.Change(0);
    }

    public void OnExitGame()
    {
        Application.Quit();
    }
}
