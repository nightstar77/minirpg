using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    #region ===== Inspector =====

    [Header("移动设置")]

    [Tooltip("玩家移动速度（单位：米/秒）")]
    public float moveSpeed = 6f;

    #endregion

    #region ===== Component =====
    private Rigidbody2D rb;
    private PlayerState playerState;

    #endregion

    #region ===== Runtime =====
    private Vector2 moveInput;
    /*
    * 玩家最后移动方向
    *
    * 用于：
    *
    * 1. 动画朝向
    * 2. 攻击方向
    * 3. WeaponPoint方向
    */
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

    /// <summary>
    /// 玩家移动
    /// </summary>
    private void Move()
    {
        if (playerState.IsState(PlayerState.State.Attack))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        rb.linearVelocity = (moveInput * moveSpeed);
    }

    #endregion

    #region ===== Property =====

    /// <summary>
    /// 当前输入方向
    /// </summary>
    public Vector2 MoveDirection => moveInput;

    /// <summary>
    /// 玩家最后朝向
    /// </summary>
    public Vector2 LastMoveDirection => lastMoveDirection;

    /// <summary>
    /// 当前是否正在移动
    /// </summary>
    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;

    #endregion
}