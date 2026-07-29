using UnityEngine;

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
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
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
    /*
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.radius);
    }
    */
}