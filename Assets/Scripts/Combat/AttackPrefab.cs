using UnityEngine;

/// <summary>
/// 攻击预制体：玩家与敌人共用的攻击碰撞体。
/// isPlayerAttack 决定命中 Player 还是 Enemy 标签。
/// </summary>
public class AttackPrefab : MonoBehaviour
{
    public float DestroyTime = 0.3f;
    public bool isPlayerAttack = true;
    public float attackDamage = 10f;
    private Vector2 attackDirection;

    void Start()
    {
        Destroy(gameObject, DestroyTime);
    }

    /// <summary>
    /// 初始化攻击参数
    /// </summary>
    /// <param name="isPlayer">是否为玩家攻击（true命中敌人，false命中玩家）</param>
    /// <param name="damage">伤害数值</param>
    /// <param name="direction">攻击方向（用于击退）</param>
    public void Init(bool isPlayer, float damage, Vector2 direction)
    {
        isPlayerAttack = isPlayer;
        attackDamage = damage;
        attackDirection = direction.normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayerAttack)
        {
            // 玩家攻击 → 命中敌人
            if (collision.CompareTag("Enemy"))
            {
                IDamageable target = collision.GetComponent<IDamageable>();
                target.TakeDamage(attackDamage, attackDirection);
            }
        }
        else
        {
            // 敌人攻击 → 命中玩家
            if (collision.CompareTag("Player"))
            {
                IDamageable target = collision.GetComponent<IDamageable>();
                target.TakeDamage(attackDamage, attackDirection);
            }
        }
    }
}
