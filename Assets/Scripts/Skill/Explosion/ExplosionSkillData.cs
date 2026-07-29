using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionSkillData", menuName = "Game/Skill/Explosion Skill Data")]
public class ExplosionSkillData : SkillData
{
    [Header("Explosion")]
    public float Damage = 30f;
    public float radius = 3f;

}