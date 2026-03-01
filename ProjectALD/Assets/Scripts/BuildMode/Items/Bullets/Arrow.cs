using UnityEngine;

public class Arrow : Item, IAttackable
{
    public BulletData data;
    private GameObject _target;
    
    private void Start()
    {
        data = LoadBulletData("ArrowSO");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            _target = collision.gameObject;
        }
    }

    public void Attack()
    {
        if (_target != null)
        {
            _target.GetComponent<IDamagable>()?.TakeDamage(data.damage);
        }
    }
}
