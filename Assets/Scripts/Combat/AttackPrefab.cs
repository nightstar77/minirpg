using UnityEngine;

namespace MiniRPG
{
    /// <summary>
    /// 攻击预制体：玩家与敌人共用的攻击碰撞体。
    /// isPlayerAttack 决定命中 Player 还是 Enemy 标签。
    ///
    /// 已接入对象池（实现 IPoolable）：通过 ObjectPool.Spawn 生成，寿命结束后归还池中。
    /// </summary>
    public class AttackPrefab : MonoBehaviour, IPoolable
    {
        public float DestroyTime = 0.3f;
        public bool isPlayerAttack = true;
        public float attackDamage = 10f;

        private Vector2 attackDirection;
        private float lifeTimer;

        private void Awake()
        {
            // 兜底：直接摆在场景里的实例也能正常计时
            lifeTimer = DestroyTime;
        }

        public void OnSpawned()
        {
            // 每次取出时重置寿命，避免沿用上一次的剩余时间
            lifeTimer = DestroyTime;
        }

        public void OnDespawned()
        {
        }

        private void Update()
        {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                ObjectPool.Despawn(gameObject);
            }
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
            // 玩家攻击 → 命中敌人；敌人攻击 → 命中玩家
            string targetTag = isPlayerAttack ? "Enemy" : "Player";
            if (!collision.CompareTag(targetTag))
            {
                return;
            }

            // 目标可能挂在父物体上，用 GetComponentInParent 更稳妥
            IDamageable target = collision.GetComponentInParent<IDamageable>();
            if (target == null)
            {
                return;
            }

            target.TakeDamage(attackDamage, attackDirection);

            // 玩家防御成功：血量未减少，不飘伤害数字，改为格挡提示
            PlayerHealth playerHealth = target as PlayerHealth;
            if (playerHealth != null && playerHealth.IsDefending)
            {
                CombatFeedback.OnBlock(collision.transform.position);
                return;
            }

            // 命中反馈：敌人打玩家时飘红字并强化震屏
            CombatFeedback.OnHit(collision.transform.position, attackDamage, !isPlayerAttack);
        }
    }

}
