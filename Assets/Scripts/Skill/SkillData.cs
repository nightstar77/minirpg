using UnityEngine;

/// <summary>
/// 技能配置基类
///
/// 所有技能配置都继承它。
/// </summary>
public abstract class SkillData : ScriptableObject
{
    [Header("基础信息")]
    public string skillName;

    [TextArea]
    public string description;

    [Header("图标")]
    public Sprite icon;

    [Header("冷却")]
    public float cooldown = 1f;
}