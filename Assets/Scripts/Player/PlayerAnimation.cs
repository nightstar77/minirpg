using UnityEngine;

/// <summary>
/// =======================================================
/// PlayerAnimation
///
/// 玩家动画模块
///
/// 【职责】
///
/// 1、读取玩家移动状态
/// 2、读取玩家状态
/// 3、控制Animator参数
///
/// 【不负责】
///
/// × 输入
/// × 移动
/// × 攻击
/// × 血量
///
/// Version 0.5
/// =======================================================
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    #region ===== Component =====

    /// <summary>
    /// Animator组件
    ///
    /// Unity动画控制器。
    /// 所有动画最终都由Animator负责播放。
    /// </summary>
    private Animator animator;

    /// <summary>
    /// 玩家移动模块
    ///
    /// 提供：
    /// 是否移动
    /// 移动方向
    /// </summary>
    private PlayerController playerController;

    /// <summary>
    /// 玩家状态机
    /// </summary>
    private PlayerState playerState;

    #endregion

    #region ===== Unity =====

    /// <summary>
    /// 游戏开始时执行一次。
    /// 缓存所有需要使用的组件。
    /// </summary>
    private void Start()
    {
        animator = GetComponent<Animator>();

        playerController = GetComponent<PlayerController>();

        playerState = GetComponent<PlayerState>();
    }

    /// <summary>
    /// 每帧刷新动画。
    /// </summary>
    private void Update()
    {
        UpdateMoveAnimation();
    }

    #endregion

    #region ===== Animation =====

    /// <summary>
    /// 更新移动动画。
    /// </summary>
    private void UpdateMoveAnimation()
    {
        // 如果角色已经死亡，不再更新移动动画。
        if (playerState.IsState(PlayerState.State.Dead))
        {
            return;
        }

        // IsMoving 是 PlayerController 提供的属性。
        // true：角色正在移动
        // false：角色静止
        float speed = playerController.IsMoving ? 1f : 0f;

        // 设置 Animator 中的 Speed 参数。
        animator.SetFloat("Speed", speed);
    }

    #endregion

    #region ===== Public =====

    /// <summary>
    /// 播放受伤动画。
    /// Trigger 类型参数。
    /// </summary>
    public void PlayHurt()
    {
        animator.SetTrigger("Hurt");
    }

    /// <summary>
    /// 播放死亡动画。
    /// Trigger 类型参数。
    /// </summary>
    public void PlayDead()
    {
        animator.SetTrigger("Dead");
    }

    /// <summary>
    /// 播放连击动画
    ///
    /// 参数：
    /// combo
    /// 当前连击编号
    ///
    /// 例如：
    /// Combo=1 → Attack1
    /// Combo=2 → Attack2
    /// Combo=3 → Attack3
    ///
    /// 以后整个项目只有这里可以修改 Animator 的 Combo 参数。
    /// </summary>
    public void PlayCombo(int combo)
    {
        animator.SetInteger("Combo", combo);
    }

    /// <summary>
    /// 重置连击。
    ///
    /// 攻击结束时调用。
    /// 将 Animator 的 Combo 参数恢复为0。
    /// </summary>
    public void ResetCombo()
    {
        animator.SetInteger("Combo", 0);
    }

    /// <summary>
    /// 播放普通攻击。
    ///
    /// Version0.6 暂时不用。
    /// 保留接口，方便以后统一调用。
    /// </summary>
    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }

    #endregion
}