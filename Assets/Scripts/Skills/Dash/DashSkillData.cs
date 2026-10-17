using UnityEngine;

/// <summary>
/// 冲刺技能配置：冲刺速度与持续时长
/// </summary>
[CreateAssetMenu(fileName = "DashSkillData", menuName = "Game/Skill/Dash Skill Data")]
public class DashSkillData : SkillData
{
    [Header("Dash")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
}