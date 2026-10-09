using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 玩家控制器：处理移动输入与冲刺逻辑。
/// 使用 InputSystem 的 OnMove 回调读取方向，FixedUpdate 中施加速度。
/// 攻击/受伤/死亡状态下禁止移动。
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region ===== Inspector =====

    [Header("移动设置")]
    public float moveSpeed = 6f;

    #region ===== Dash =====
    private float currentDashSpeed;
    private float dashTimer;
    private bool isDashing;
    private Vector2 dashDirection;
    #endregion

    #endregion

    #region ===== Component =====

    private Rigidbody2D rb;
    private PlayerState playerState;

    #endregion

    #region ===== Runtime =====
    private Vector2 moveInput;
    // 记录最后一次有效移动方向，冲刺和技能需要使用
    private Vector2 lastMoveDirection = Vector2.down;

    #endregion

    #region ===== Unity =====

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerState = GetComponent<PlayerState>();
    }

    private void Update()
    {
        UpdateDash();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #endregion

    #region ===== Input =====
    /// <summary>
    /// InputSystem 回调：读取 Vector2 移动输入并缓存
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // 斜向输入归一化，避免对角线速度更快
        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }

        // 记录最后一次有效方向（用于冲刺和技能释放方向）
        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = moveInput;
        }

    }

    #endregion

    #region ===== Movement =====

    private void Move()
    {
        // 冲刺中由冲刺逻辑接管速度
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * currentDashSpeed;
            return;
        }
        // 攻击/受伤/死亡时禁止移动
        if (playerState.IsState(PlayerState.State.Attack) || playerState.IsState(PlayerState.State.Hurt) || playerState.IsState(PlayerState.State.Dead))
        { return; }
        rb.linearVelocity = (moveInput * moveSpeed);
    }

    private void UpdateDash()
    {
        if (!isDashing)
        {
            return;
        }
        dashTimer -= Time.deltaTime;
        if (dashTimer <= 0)
        {
            EndDash();
        }
    }

    /// <summary>
    /// 由 DashSkill 调用，按指定方向/速度/时长开始冲刺
    /// </summary>
    public void StartDash(DashContext context)
    {
        isDashing = true;
        dashDirection = context.direction.normalized;
        currentDashSpeed = context.speed;
        dashTimer = context.duration;
    }

    public void EndDash()
    {
        isDashing = false;
    }

    #endregion

    #region ===== Property =====

    public Vector2 MoveDirection => moveInput;
    public Vector2 LastMoveDirection => lastMoveDirection;
    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;

    #endregion
}