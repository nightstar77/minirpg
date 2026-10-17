using UnityEngine;

/// <summary>
/// 火球技能：沿玩家最后移动方向发射投射体，命中后触发携带的技能效果
/// </summary>
public class FireballSkill : SkillBase
{
    [SerializeField]
    private FireballSkillData data;
    protected override SkillData Data => data;

    private Transform firePoint;

    private PlayerController playerController;

    private void Awake()
    {
        firePoint = transform;
        playerController = GetComponent<PlayerController>();
    }

    public override void Use()
    {
        if (!CanUse())
        {
            Debug.Log("Fireball冷却中");
            return;
        }
        Debug.Log("Fireball释放");

        StartCooldown();
        // 以玩家最后移动方向作为火球飞行方向
        Vector2 direction = playerController.LastMoveDirection;

        GameObject fireball = Instantiate(data.projectilePrefab, firePoint.position, Quaternion.identity);

        FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();

        projectile.Init(direction, data.speed, data.lifetime, data.effects);
    }
}