using UnityEngine;

/// <summary>
/// 爆炸技能：以施法者为中心圆形范围伤害，生成爆炸特效预制体
/// </summary>
public class ExplosionSkill : SkillBase
{
    [SerializeField]
    private ExplosionSkillData data;

    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private LayerMask enemyLayer;

    protected override SkillData Data
    {
        get { return data; }
    }

    public override void Use()
    {
        if (!CanUse())
        {
            Debug.Log("Explosion冷却中");
            return;
        }
        Debug.Log("释放Explosion");
        CreateExplosion();
        StartCooldown();
    }

    private void CreateExplosion()
    {
        // 生成爆炸特效
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        // 范围内所有敌人受伤，击退方向为爆炸中心→敌人
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, data.radius, enemyLayer);
        foreach (Collider2D enemy in enemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(data.Damage, enemy.transform.position - transform.position);
            }
        }
    }
}