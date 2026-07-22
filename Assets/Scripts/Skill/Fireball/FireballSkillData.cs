using UnityEngine;

[CreateAssetMenu(fileName = "FireballSkillData", menuName = "Game/Skill/Fireball Skill Data")]
public class FireballSkillData : SkillData
{
    [Header("Fireball")]
    public GameObject projectilePrefab;

    public float speed = 8f;
    public float lifetime = 3f;

    public SkillEffect[] effects;
}