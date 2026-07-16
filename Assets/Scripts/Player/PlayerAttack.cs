using UnityEngine;

/// <summary>
/// 玩家攻击模块
///
/// 【职责】
/// 1. 接收攻击输入
/// 2. 控制攻击流程
/// 3. 控制连击
/// 4. 通知动画播放
/// 5. 修改玩家状态
///
/// 【不负责】
/// ✘ 检测敌人
/// ✘ 造成伤害
/// ✘ 播放特效
/// ✘ 播放音效
///
/// Version 0.5
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

    /// <summary>
    /// 玩家状态机
    /// </summary>
    private PlayerState playerState;

    #endregion

    #region ===== Runtime =====

    /// <summary>
    /// 当前是否正在攻击
    /// </summary>
    private bool isAttacking;

    /// <summary>
    /// 当前连击序号
    /// Attack1=1
    /// Attack2=2
    /// </summary>
    private int comboIndex;

    /// <summary>
    /// 当前是否允许继续连击
    /// 动画事件开启
    /// </summary>
    private bool canCombo;

    /// <summary>
    /// 玩家是否已经输入下一次攻击
    /// </summary>
    private bool attackQueued;

    #endregion

    #region ===== Unity =====

    private void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        playerState = GetComponent<PlayerState>();
    }

    private void Update()
    {
        ReadAttackInput();
    }

    #endregion

    #region ===== Input =====

    /// <summary>
    /// 读取攻击输入
    /// </summary>
    private void ReadAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleAttackInput();
        }
    }

    /// <summary>
    /// 处理攻击输入
    /// </summary>
    private void HandleAttackInput()
    {
        if (isAttacking)
        {
            if (canCombo)
            {
                attackQueued = true;
            }

            return;
        }

        StartComboAttack();
    }

    #endregion

    #region ===== Combo =====

    /// <summary>
    /// 开始一次攻击
    /// </summary>
    private void StartComboAttack()
    {
        isAttacking = true;
        playerState.ChangeState(PlayerState.State.Attack);
        comboIndex++;
        if (comboIndex > maxCombo)
        {
            comboIndex = 1;
        }
        playerAnimation.PlayCombo(comboIndex);
    }

    /// <summary>
    /// 动画事件调用
    /// 开启连击窗口
    /// </summary>
    public void EnableCombo()
    {
        canCombo = true;
    }

    /// <summary>
    /// 动画事件调用
    /// 攻击结束
    /// </summary>
    public void EndComboAttack()
    {
        if (attackQueued)
        {
            attackQueued = false;
            canCombo = false;
            StartComboAttack();
            return;
        }
        isAttacking = false;
        comboIndex = 0;
        canCombo = false;
        playerState.ChangeState(PlayerState.State.Idle);
        playerAnimation.ResetCombo();
    }

    #endregion

    #region ===== Property =====

    /// <summary>
    /// 当前是否正在攻击
    /// 提供给其它脚本读取
    /// </summary>
    public bool IsAttacking => isAttacking;

    #endregion
}