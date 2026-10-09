using UnityEngine;

/// <summary>
/// 火球技能配置：投射体预制体、飞行速度、生命周期和携带的技能效果数组
/// </summary>
[CreateAssetMenu(fileName = "FireballSkillData", menuName = "Game/Skill/Fireball Skill Data")]
public class FireballSkillData : SkillData
{
    [Header("Fireball")]
    public GameObject projectilePrefab;

    public float speed = 8f;
    public float lifetime = 3f;

    // 命中敌人后依次应用的效果（可组合伤害、减速等）
    public SkillEffect[] effects;
}