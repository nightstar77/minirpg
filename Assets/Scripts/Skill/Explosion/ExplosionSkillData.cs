using UnityEngine;

/// <summary>
/// 爆炸技能配置：伤害值与爆炸半径
/// </summary>
[CreateAssetMenu(fileName = "ExplosionSkillData", menuName = "Game/Skill/Explosion Skill Data")]
public class ExplosionSkillData : SkillData
{
    [Header("Explosion")]
    public float Damage = 30f;
    public float radius = 3f;

}