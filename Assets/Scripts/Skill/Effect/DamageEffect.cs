using UnityEngine;

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
    }
}