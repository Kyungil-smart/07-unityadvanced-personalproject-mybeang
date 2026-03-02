using System.Collections;
using UnityEngine;

public class EnemyStatus : MonoBehaviour, IDamagable, IDead
{
    public bool isDead;
    public MonsterData data;
    private WaitForSeconds wait;
    private Animator anim;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        wait = new WaitForSeconds(1f);
    }
    
    public void UpdateStatus(MonsterData data)
    {
        this.data = data;
    }

    public void TakeDamage(float damage, DamageType damageType)
    {
        data.HP -= (int)damage;
        if (data.HP <= 0) StartCoroutine(Dead());
    }
    public IEnumerator Dead()
    {
        isDead = true;
        // ToDo. Object Pool
        anim.SetTrigger("DeathTrigger");
        yield return wait;
        Destroy(gameObject);
    }
}