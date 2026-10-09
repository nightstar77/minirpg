using UnityEngine;

/// <summary>
/// 技能类型枚举，用于技能注册、查找和存档
/// </summary>
public enum SkillType { None, Dash, Fireball, Explosion, Defense }

/// <summary>
/// 技能配置基类（ScriptableObject）：所有技能配置的基类，在 Inspector 中创建和配置
/// </summary>
public abstract class SkillData : ScriptableObject
{
    public SkillType skillType;

    [Header("基础信息")]
    public string skillName;

    [TextArea]
    public string description;

    [Header("图标")]
    public Sprite icon;

    [Header("冷却")]
    public float cooldown = 1f;
}