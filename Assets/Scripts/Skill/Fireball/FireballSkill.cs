using UnityEngine;

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
        Debug.Log("Fireball释放");
        if (!CanUse())
        {
            return;
        }

        StartCooldown();
        Vector2 direction = playerController.LastMoveDirection;

        GameObject fireball = Instantiate(data.projectilePrefab, firePoint.position, Quaternion.identity);

        FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();

        projectile.Init(direction, data.damage, data.speed);
    }
}