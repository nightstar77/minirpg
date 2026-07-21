using UnityEngine;

public class DashSkill : SkillBase
{
    [SerializeField]
    private DashSkillData data;
    protected override SkillData Data => data;

    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public override void Use()
    {
        if (!CanUse()) { return; }
        StartCooldown();
        DashContext context = new DashContext(playerController.LastMoveDirection, data.dashSpeed, data.dashDuration);
        playerController.StartDash(context);
    }

}