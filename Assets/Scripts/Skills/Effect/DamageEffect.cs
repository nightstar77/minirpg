using UnityEngine;

namespace MiniRPG
{
    /// <summary>
    /// 伤害效果：对目标施加固定伤害，方向来自技能上下文（用于击退）
    /// </summary>
    [CreateAssetMenu(fileName = "DamageEffect", menuName = "Game/Skill/Effect/Damage")]
    public class DamageEffect : SkillEffect
    {
        public float damage = 20f;

        public override void Apply(IDamageable target, SkillContext context)
        {
            target.TakeDamage(damage, context.direction);

            // 命中反馈（停顿 / 震屏 / 飘字），与近战命中走同一入口，
            // 避免火球等远程技能命中时缺少伤害飘字。
            Vector3 hitPos = (target as MonoBehaviour)?.transform.position ?? context.position;
            CombatFeedback.OnHit(hitPos, damage, false);
        }
    }
}
