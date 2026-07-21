using UnityEngine;

public class AttackPrefeb : MonoBehaviour
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
    /// 初始化
    /// </summary>
    /// <param name="isPlayer">是否为玩家攻击</param>
    /// <param name="damage">伤害数值</param>
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
            if (collision.CompareTag("Enemy"))
            {
                IDamageable target = collision.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(attackDamage, attackDirection);
                }
            }
        }
        else
        {
            if (collision.CompareTag("Player"))
            {
                IDamageable target = collision.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(attackDamage, attackDirection);
                }
            }
        }
    }
}
