using UnityEngine;

public class RestartUI : MonoBehaviour
{
    private void Start()
    {
        SceneChangeManager.Instance.Change(1);
    }
}
