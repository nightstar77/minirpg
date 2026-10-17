using UnityEngine;

/// <summary>
/// 敌人动画控制器。
///
/// 项目规定：
///
/// EnemyBase 不允许直接操作 Animator。
///
/// 所有动画统一由 EnemyAnimation 控制。
/// </summary>
public class EnemyAnimation : MonoBehaviour
{
    #region ===== Component =====

    private Animator animator;

    #endregion

    #region ===== Unity =====

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    #endregion

    #region ===== Public =====

    /// <summary>
    /// 设置移动动画
    /// </summary>
    public void SetMove(bool moving)
    {
        animator.SetBool("IsRun", moving);
    }

    /// <summary>
    /// 播放攻击动画
    /// </summary>
    public void PlayAttack()
    {
        animator.SetTrigger("Attack1");
    }

    /// <summary>
    /// 播放受伤动画
    /// </summary>
    public void PlayHurt()
    {
        animator.SetTrigger("GetHit");
    }

    /// <summary>
    /// 播放死亡动画
    /// </summary>
    public void PlayDead()
    {
        animator.SetTrigger("Dead");
    }

    #endregion
}