using UnityEngine;

public class Melee : Monster, IAttackable, IDamagable
{
    public string scriptableObjectName;
    private void Start()
    {
        data = LoadMonsterData(scriptableObjectName);
    }
    
    public void Attack()
    {
        
    }

    public void TakeDamage(float damage)
    {
        
    }
}
