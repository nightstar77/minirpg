using UnityEngine;

/// <summary>
/// 玩家攻击控制器：管理连击输入队列、HitBox 开关时机与动画协调。
/// J键攻击，K键取消攻击。连击段数循环（超过 maxCombo 回到第1段）。
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    #region ===== Inspector =====
    [Header("Combo 设置")]
    [Tooltip("最大连击段数")]
    public int maxCombo = 2;

    #endregion

    #region ===== Component =====
    private PlayerAnimation playerAnimation;
    private PlayerState playerState;
    private Rigidbody2D rb;
    [SerializeField]
    private WeaponHitBox weapon;

    #endregion

    #region ===== Runtime =====
    private bool isAttacking;     // 是否正在攻击动画中
    private int comboIndex;       // 当前连击段数（1~maxCombo）
    private bool canCombo;        // 连击窗口是否开启（由动画事件 EnableCombo 开启）
    private bool attackQueued;    // 连击窗口内玩家是否输入了下一段攻击

    #endregion

    #region ===== Unity =====

    private void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        playerState = GetComponent<PlayerState>();
        rb = GetComponent<Rigidbody2D>();
        weapon = GetComponentInChildren<WeaponHitBox>();
        weapon.Init(transform);
    }

    private void Update()
    {
        ReadAttackInput();
    }

    #endregion

    #region ===== Input =====
    /// <summary>
    /// 动画事件回调：攻击开始时开启 HitBox
    /// </summary>
    public void StartHitBox()
    {
        if (weapon == null)
        {
            Debug.LogError("PlayerAttack没有绑定WeaponHitBox");
            return;
        }
        if (!playerState.IsState(PlayerState.State.Attack))
        {
            Debug.Log("当前不是攻击状态，取消HitBox");
            return;
        }
        weapon.ResetHitTargets();
        weapon.EnableHitBox();
    }

    /// <summary>
    /// 动画事件回调：攻击结束时关闭 HitBox
    /// </summary>
    public void EndHitBox()
    {
        if (weapon == null)
        {
            return;
        }
        weapon.DisableHitBox();
    }

    private void ReadAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            HandleAttackInput();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            CancelAttack();
        }
    }

    private void HandleAttackInput()
    {
        // 防御中不能攻击
        if (playerState.IsState(PlayerState.State.Defend))
        { return; }
        // 正在攻击中：如果连击窗口开启，缓存输入等待下一段
        if (isAttacking)
        {
            if (canCombo)
            { attackQueued = true; }
            return;
        }
        StartComboAttack();
    }

    #endregion

    #region ===== Combo =====
    private void StartComboAttack()
    {
        isAttacking = true;
        playerState.ChangeState(PlayerState.State.Attack);
        rb.linearVelocity = Vector2.zero;
        comboIndex++;
        // 超过最大连击数，回到第1段
        if (comboIndex > maxCombo)
        {
            comboIndex = 1;
        }
        playerAnimation.PlayCombo(comboIndex);
    }

    /// <summary>
    /// 动画事件回调：开启连击输入窗口
    /// </summary>
    public void EnableCombo()
    {
        canCombo = true;
    }

    /// <summary>
    /// 动画事件回调：当前段攻击结束，判断是否有缓存的连击输入
    /// </summary>
    public void EndComboAttack()
    {
        // 有缓存的连击输入 → 继续下一段攻击
        if (attackQueued)
        {
            attackQueued = false;
            canCombo = false;
            StartComboAttack();
            return;
        }
        // 无连击输入 → 结束攻击
        isAttacking = false;
        comboIndex = 0;
        canCombo = false;
        playerState.ChangeState(PlayerState.State.Idle);
        playerAnimation.ResetCombo();
    }

    /// <summary>
    /// 强制取消当前攻击
    /// 使用场景：1.玩家受伤 2.玩家死亡 3.技能打断
    /// </summary>
    public void CancelAttack()
    {
        if (weapon != null)
        {
            weapon.DisableHitBox();
        }
        isAttacking = false;
        comboIndex = 0;
        attackQueued = false;
        canCombo = false;
        playerAnimation.ResetCombo();
        playerState.ChangeState(PlayerState.State.Idle);
        Debug.Log("攻击被取消");
    }

    #endregion

    #region ===== Property =====

    /// <summary>
    /// 当前是否正在攻击，供其他脚本读取
    /// </summary>
    public bool IsAttacking => isAttacking;

    #endregion
}