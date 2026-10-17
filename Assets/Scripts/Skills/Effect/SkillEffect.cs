using UnityEngine;

/// <summary>
/// 技能效果基类（ScriptableObject）：子类实现 Apply 方法对目标施加效果。
/// 可组合多个效果（如伤害+减速）挂载到技能预制体上。
/// </summary>
public abstract class SkillEffect : ScriptableObject
{
    public abstract void Apply(IDamageable target, SkillContext context);
}