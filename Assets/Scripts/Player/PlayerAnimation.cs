using UnityEngine;

/// <summary>
/// 玩家动画控制器：根据移动输入和状态播放对应动画，管理朝向翻转
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    #region ===== Component =====

    private Animator animator;
    private PlayerController playerController;
    private PlayerState playerState;
    private SpriteRenderer spriteRenderer;

    #endregion

    #region ===== Unity =====
    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        playerState = GetComponent<PlayerState>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        UpdateMoveAnimation();
        UpdateFacing();
    }

    #endregion

    #region ===== Animation =====
    private void UpdateMoveAnimation()
    {
        // 死亡或防御状态下不更新移动动画
        if (playerState.IsState(PlayerState.State.Dead) || playerState.IsState(PlayerState.State.Defend))
        { return; }

        // 用移动输入的模长作为 Animator Speed 参数
        float speed = playerController.MoveDirection.magnitude;
        animator.SetFloat("Speed", speed);
    }

    private void UpdateFacing()
    {
        Vector2 direction = playerController.MoveDirection;

        if (direction.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (direction.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    #endregion

    #region ===== Public =====

    public void PlayHurt()
    {
        animator.SetTrigger("Hurt");
    }

    public void PlayDead()
    {
        animator.SetTrigger("Dead");
    }

    /// <summary>
    /// 设置 Animator Combo 整数参数，驱动连击动画切换
    /// </summary>
    public void PlayCombo(int combo)
    {
        animator.SetInteger("Combo", combo);
    }

    public void ResetCombo()
    {
        animator.SetInteger("Combo", 0);
    }

    public void PlayDefend()
    {
        animator.SetBool("Defend", true);
    }

    public void StopDefend()
    {
        animator.SetBool("Defend", false);
    }

    /// <summary>
    /// 设置动画全局播放速度，用于攻击速度强化。
    /// multiplier = 1.0 为正常速度，>1 整体加快（含攻击、移动等）。
    /// 若只想让攻击动画加速而不影响移动，可在 Animator 攻击 clip 的
    /// Speed 上勾选 Multiplier 指向 AttackSpeed 参数，并改用 SetFloat 设置。
    /// </summary>
    public void SetAnimationSpeed(float multiplier)
    {
        animator.speed = multiplier;
    }

    #endregion
}