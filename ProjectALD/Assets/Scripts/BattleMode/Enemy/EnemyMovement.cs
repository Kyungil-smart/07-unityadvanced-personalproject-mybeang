using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public bool CanMove;
    private Vector3 targetPos;
    private EnemyStatus status;
    private Animator anim;
    private bool isMoving = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    
    private void Start()
    {
        targetPos = new Vector3(GridManager.Instance.WallPosX - 0.1f, transform.position.y, transform.position.z);
        status = gameObject.GetComponent<EnemyStatus>();
    }
    
    private void Update()
    {
        if (CanMove && Mathf.Abs(targetPos.x - transform.position.x) >= status.data.attackRange)
            Move();
        else isMoving = false;
    }
    
    private void Move()
    {
        if (!isMoving) anim.SetTrigger("MoveTrigger");
        transform.position = Vector3.MoveTowards(
            transform.position, targetPos, status.data.moveSpeed * Time.deltaTime);
        isMoving = true;
    }
}
