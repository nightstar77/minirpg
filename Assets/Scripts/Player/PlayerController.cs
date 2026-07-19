using UnityEngine;
using UnityEngine.InputSystem;
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
    private Vector2 lastMoveDirection = Vector2.down;

    #endregion

    #region ===== Unity =====

    /// <summary>
    /// Start
    ///
    /// Unity生命周期函数。
    ///
    /// 在游戏开始时执行一次。
    ///
    /// 用于缓存组件。
    /// </summary>
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
    public void OnMove(InputAction.CallbackContext context)
    {

        /*
         * ReadValue<Vector2>()
         *
         * 读取输入值
         *
         * 因为Move设置的是Vector2
         *
         * 所以读取Vector2
         */
        moveInput = context.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }

        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = moveInput;
        }

    }

    #endregion

    #region ===== Movement =====

    private void Move()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * currentDashSpeed;
            return;
        }
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

    public void StartDash(Vector2 direction, float speed, float duration)
    {
        isDashing = true;
        dashDirection = direction.normalized;
        currentDashSpeed = speed;
        dashTimer = duration;
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