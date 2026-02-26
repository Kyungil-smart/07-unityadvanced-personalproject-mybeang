using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    private void Awake()
    {
        SingleTonInit();
    }
}
