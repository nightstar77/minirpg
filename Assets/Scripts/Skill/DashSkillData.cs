using UnityEngine;

[CreateAssetMenu(fileName = "DashSkillData", menuName = "Game/Skill/Dash Skill Data")]
public class DashSkillData : SkillData
{
    [Header("Dash")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
}