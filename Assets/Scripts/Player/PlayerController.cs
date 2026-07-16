using UnityEngine;

/// <summary>
/// ===========================================================
/// PlayerController
///
/// 玩家移动模块
///
/// 【职责】
///
/// 1、读取玩家输入
/// 2、计算移动方向
/// 3、CharacterController移动
/// 4、控制角色朝向
///
/// 【不负责】
///
/// × 攻击
/// × 动画
/// × 血量
/// × 技能
///
/// Version 0.5
/// ===========================================================
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region ===== Inspector =====

    [Header("移动设置")]

    [Tooltip("玩家移动速度（单位：米/秒）")]
    public float moveSpeed = 6f;

    #endregion

    #region ===== Component =====

    /// <summary>
    /// CharacterController组件
    ///
    /// Unity官方提供的人物控制器。
    ///
    /// 用于：
    ///     Move()
    ///     碰撞检测
    ///     楼梯
    ///     斜坡
    /// </summary>
    private CharacterController controller;

    /// <summary>
    /// SpriteRenderer
    ///
    /// 控制角色左右翻转。
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// PlayerState
    ///
    /// 控制玩家状态。
    /// </summary>
    private PlayerState playerState;

    #endregion

    #region ===== Runtime =====

    /// <summary>
    /// 输入方向
    ///
    /// x：
    ///     左右
    ///
    /// z：
    ///     上下（如果是3D）
    ///
    /// y：
    ///     上下（如果是2D）
    /// </summary>
    private Vector3 moveDirection;

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
    private void Start()
    {
        controller = GetComponent<CharacterController>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        playerState = GetComponent<PlayerState>();
    }

    /// <summary>
    /// Update
    ///
    /// Unity每帧调用。
    /// </summary>
    private void Update()
    {
        ReadMovementInput();

        Move();
    }

    #endregion

    #region ===== Input =====

    /// <summary>
    /// 读取玩家输入
    /// </summary>
    private void ReadMovementInput()
    {
        // 如果正在攻击，不允许移动（后续可根据需要改成允许缓慢移动）
        if (playerState.IsState(PlayerState.State.Attack))
        {
            moveDirection = Vector3.zero;
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");

        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(horizontal, vertical, 0f);

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }
    }

    #endregion

    #region ===== Movement =====

    /// <summary>
    /// 玩家移动
    /// </summary>
    private void Move()
    {
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        UpdateFacingDirection();
    }

    /// <summary>
    /// 更新角色朝向
    /// </summary>
    private void UpdateFacingDirection()
    {
        if (moveDirection.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveDirection.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    #endregion

    #region ===== Property =====

    /// <summary>
    /// 给其它模块读取移动方向
    /// </summary>
    public Vector3 MoveDirection => moveDirection;

    /// <summary>
    /// 当前是否正在移动
    /// </summary>
    public bool IsMoving => moveDirection.sqrMagnitude > 0.01f;

    #endregion
}