using UnityEngine;

/// <summary>
/// 防御技能配置
/// </summary>
[CreateAssetMenu(fileName = "DefenseSkillData", menuName = "Game/Skill/Defense Skill Data")]
public class DefenseSkillData : SkillData
{
    [Header("Defense")]
    public bool isGuard;

}