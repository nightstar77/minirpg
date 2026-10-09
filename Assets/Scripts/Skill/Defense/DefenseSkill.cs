using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 防御技能：持续 defendTime 秒内免疫伤害，结束后恢复
/// </summary>
public class DefenseSkill : SkillBase
{
    [SerializeField]
    private SkillData data;

    private PlayerHealth playerHealth;
    private PlayerAnimation playerAnimation;
    private PlayerState playerState;
    public float defendTime = 1f;

    protected override SkillData Data
    {
        get { return data; }
    }

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerState = GetComponent<PlayerState>();
    }

    public override void Use()
    {
        if (!CanUse())
        {
            Debug.Log("防御冷却中");
            return;
        }
        StartDefend();
        StartCooldown();
    }

    private void StartDefend()
    {
        playerHealth.StartDefend();
        playerAnimation.PlayDefend();
        playerState.ChangeState(PlayerState.State.Defend);
        // defendTime 秒后自动结束防御
        Invoke(nameof(EndDefend), defendTime);
    }

    private void EndDefend()
    {
        playerHealth.EndDefend();
        playerAnimation.StopDefend();
        // 仅在仍处于防御状态时恢复到 Idle
        if (playerState.IsState(PlayerState.State.Defend))
        {
            playerState.ChangeState(PlayerState.State.Idle);
        }
    }

}