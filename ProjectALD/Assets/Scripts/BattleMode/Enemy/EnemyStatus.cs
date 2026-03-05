using System.Collections;
using UnityEngine;

public class EnemyStatus : MonoBehaviour, IDamagable, IDead
{
    public bool isDead;
    public MonsterData data;
    private WaitForSeconds wait;
    private Animator anim;
    private float _hp;
    private float _maxHp;
    private string _soName;
    private float _speed;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        wait = new WaitForSeconds(1.5f);
    }

    private void OnEnable()
    {
        anim.Rebind();
    }
    
    public void UpdateStatus(MonsterData data, float hp, string soName)
    {
        this.data = data;
        _hp = hp;
        _maxHp = hp;
        _soName = soName;
    }

    public void TakeDamage(float damage, DamageType damageType)
    {
        // 시간되면 damageType 에 따른 연산 추가하기.
        _hp -= damage;
        if (_hp <= 0) StartCoroutine(Dead());
    }
    public IEnumerator Dead()
    {
        PlayerStatusManager.Instance.EarnGold((int)_maxHp);
        isDead = true;
        SpawanManager.Instance.DecreseSpawnCount(_soName);
        anim.SetTrigger("DeathTrigger");
        yield return wait;
        ObjectPoolManager.Instance.PushGameObject(gameObject);
    }
}