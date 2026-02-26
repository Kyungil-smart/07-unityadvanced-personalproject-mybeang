using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    public bool IsPause;
    
    private void Awake()
    {
        SingleTonInit();
    }
}
