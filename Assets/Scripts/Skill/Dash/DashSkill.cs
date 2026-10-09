using UnityEngine;

/// <summary>
/// 冲刺技能：沿玩家最后移动方向高速冲刺，持续 dashDuration 秒
/// </summary>
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
        // 用玩家最后移动方向构造冲刺参数
        DashContext context = new DashContext(playerController.LastMoveDirection, data.dashSpeed, data.dashDuration);
        playerController.StartDash(context);
    }

}