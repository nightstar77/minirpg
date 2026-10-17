using UnityEngine;

/// <summary>
/// 玩家防御控制器：与 PlayerAttack 同模式的常驻基础能力，
/// 不走技能树/技能槽/冷却系统。
/// 按住防御键（默认 K，可在设置面板改键）进入防御状态（免疫伤害），松开解除。
/// 防御期间不能移动、不能攻击。
/// </summary>
public class PlayerDefense : MonoBehaviour
{
    #region ===== Component =====
    private PlayerAnimation playerAnimation;
    private PlayerState playerState;
    private PlayerHealth playerHealth;
    private Rigidbody2D rb;

    #endregion

    #region ===== Runtime =====
    private bool isDefending;

    #endregion

    #region ===== Unity =====

    private void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        playerState = GetComponent<PlayerState>();
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 改键捕获期间暂停游戏输入
        if (GameInputs.IsCapturing)
        {
            return;
        }

        if (Input.GetKeyDown(GameInputs.GetKey(GameInputs.Action.Defend)))
        {
            StartDefend();
        }
        if (Input.GetKeyUp(GameInputs.GetKey(GameInputs.Action.Defend)))
        {
            StopDefend();
        }
    }

    #endregion

    #region ===== Defend =====

    private void StartDefend()
    {
        // 已在防御中，或处于攻击/受伤/死亡等不可防御的状态时不响应
        if (isDefending) { return; }
        if (playerState.IsState(PlayerState.State.Attack) ||
            playerState.IsState(PlayerState.State.Hurt) ||
            playerState.IsState(PlayerState.State.Dead))
        { return; }

        isDefending = true;
        rb.linearVelocity = Vector2.zero;
        playerState.ChangeState(PlayerState.State.Defend);
        playerHealth.StartDefend();
        playerAnimation.PlayDefend();
    }

    private void StopDefend()
    {
        if (!isDefending) { return; }
        isDefending = false;
        playerHealth.EndDefend();
        playerAnimation.StopDefend();
        // 仅在仍处于防御状态时恢复到 Idle（状态被其他逻辑接管时不覆盖）
        if (playerState.IsState(PlayerState.State.Defend))
        {
            playerState.ChangeState(PlayerState.State.Idle);
        }
    }

    /// <summary>
    /// 强制解除防御（受击打断、死亡等场景由外部调用）
    /// </summary>
    public void CancelDefend()
    {
        StopDefend();
    }

    #endregion

    #region ===== Property =====

    /// <summary>
    /// 当前是否正在防御，供其他脚本读取
    /// </summary>
    public bool IsDefending => isDefending;

    #endregion
}
