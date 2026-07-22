using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Game/Skill/Effect/Damage")]
public class DamageEffect : SkillEffect
{
    public float damage = 20f;

    public override void Apply(IDamageable target, SkillContext context)
    {
        target.TakeDamage(damage, context.direction);
    }
}