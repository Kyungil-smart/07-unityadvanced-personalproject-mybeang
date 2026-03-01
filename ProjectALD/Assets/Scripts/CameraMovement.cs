using UnityEngine;
using UnityEngine.Events;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform buildFieldTransform;
    [SerializeField] Transform battleFieldTransform;
    private Vector3 _battleFieldPosition;
    private Vector3 _buildFieldPosition;
    private bool _isBattleField;
    public bool IsBattleField
    {
        get { return _isBattleField; }
        set
        {
            _isBattleField = value;
        }
    }

    private void Start()
    {
        _battleFieldPosition = battleFieldTransform.position;
        _buildFieldPosition = buildFieldTransform.position;
    }

    private void Update()
    {
        ChangeCamera();
    }

    public void ChangeCamera() => 
        transform.position = Vector3.Lerp(
            transform.position, 
            _isBattleField ? _battleFieldPosition : _buildFieldPosition, 
            Time.deltaTime * 5f);
}
