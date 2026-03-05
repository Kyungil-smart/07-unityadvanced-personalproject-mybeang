using UnityEngine.SceneManagement;

public class SceneChangeManager : SingleTon<SceneChangeManager>
{
    private void Awake()
    {
        SingleTonInit();
    }
    
    public void Change(int index)
    {
        SceneManager.LoadScene(index);
    }
}
