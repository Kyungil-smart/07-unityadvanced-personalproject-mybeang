using UnityEngine;

/// <summary>
/// SingleTon 을 활용해야 하는 Manager 들인 본 Class 를 상속받기 바랍니다.
/// </summary>
/// <typeparam name="T">Generic Type</typeparam>
public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
            }
            DontDestroyOnLoad(_instance.gameObject);
            return _instance;
        }
    }
    
    protected void SingleTonInit()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this as T;
            DontDestroyOnLoad(_instance.gameObject);
        }
    }
}