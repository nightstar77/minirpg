using UnityEngine;
using System.Collections.Generic;

namespace MiniRPG
{
    /// <summary>
    /// 武器碰撞盒：挂在玩家武器子对象上，控制攻击判定的开启/关闭与伤害触发。
    /// 通过 HashSet 记录本次挥击已命中目标，避免同一攻击多次伤害同一敌人。
    /// </summary>
    public class WeaponHitBox : MonoBehaviour
    {
        #region ===== Inspector =====
        [Header("攻击设置")]
        public float damage = 10;

        [Header("命中层级")]
        [Tooltip("武器可命中的层（如 Enemy、Item）。在 Inspector 里勾选对应层。")]
        public LayerMask targetLayers;
        #endregion

        #region ===== Component =====
        private Collider2D hitBox;
        // Inspector 中配置的判定盒偏移，作为“朝右”时的基准值
        private Vector2 baseOffset;
        #endregion

        #region Runtime
        // 本次挥击已命中的目标集合，每次开启 HitBox 前需清空
        private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
        // 攻击者 Transform，用于计算攻击方向（击退）
        private Transform attacker;
        #endregion

        #region ===== Unity =====
        private void Awake()
        {
            hitBox = GetComponent<Collider2D>();
            baseOffset = hitBox.offset;
            DisableHitBox();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 排除攻击者自身（武器 HitBox 可能与玩家自身碰撞体重叠）
            if (attacker != null && other.transform.root == attacker.root)
            { return; }

            // 只命中目标层级（Enemy、Item 等）
            if (!IsInTargetLayer(other.gameObject.layer))
            {
                return;
            }

            // 命中规则：目标需实现 IDamageable（敌人、金矿等）
            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target == null)
            {
                return;
            }
            // 防止同一攻击重复命中同一目标
            if (hitTargets.Contains(target))
            {
                return;
            }
            hitTargets.Add(target);
            if (attacker == null)
            {
                Logger.LogWarning("WeaponHitBox没有初始化攻击者");
                return;
            }
            // 计算从攻击者到被击者的方向，用于击退
            Vector2 attackDirection = (other.transform.position - attacker.position).normalized;
            target.TakeDamage(damage, attackDirection);

            // 命中反馈（停顿 / 震屏 / 飘字）只对敌人触发，避免打金矿时飘出与实际耐久无关的数字
            if (other.CompareTag("Enemy"))
            {
                CombatFeedback.OnHit(other.transform.position, damage, false);
            }

            Logger.Log("攻击检测到：" + other.name);
        }
        #endregion

        #region ===== Public =====
        /// <summary>
        /// 检查指定层是否在 targetLayers 中
        /// </summary>
        private bool IsInTargetLayer(int layer)
        {
            return (targetLayers.value & (1 << layer)) != 0;
        }

        /// <summary>
        /// 初始化攻击者引用，在 PlayerAttack.Awake 中调用
        /// </summary>
        public void Init(Transform owner)
        {
            attacker = owner;
        }

        /// <summary>
        /// 清空已命中目标列表，每次新挥击开始前调用
        /// </summary>
        public void ResetHitTargets()
        {
            hitTargets.Clear();
            Logger.Log("HitBox伤害列表重置");
        }

        public void EnableHitBox()
        {
            hitBox.enabled = true;
            Logger.Log("HitBox 开启");
        }

        public void DisableHitBox()
        {
            hitBox.enabled = false;
            Logger.Log("HitBox关闭");
        }

        /// <summary>
        /// 跟随玩家朝向镜像判定盒位置：朝右保持 Inspector 中的偏移，朝左把 X 偏移取反。
        /// 玩家转身只翻 SpriteRenderer.flipX，Transform 不变，所以需要在这里主动同步。
        /// </summary>
        public void SetFacing(float facingX)
        {
            if (hitBox == null || facingX == 0)
            {
                return;
            }
            Vector2 offset = baseOffset;
            offset.x = facingX > 0 ? baseOffset.x : -baseOffset.x;
            hitBox.offset = offset;
        }
        #endregion
    }
}
