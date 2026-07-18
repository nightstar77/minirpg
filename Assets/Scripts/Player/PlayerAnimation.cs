using UnityEngine;

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
        //Debug.Log(playerState.CurrentState);
        UpdateMoveAnimation();
        UpdateFacing();
    }

    #endregion

    #region ===== Animation =====
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
        float speed = playerController.MoveDirection.magnitude;

        // 设置 Animator 中的 Speed 参数。
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

    public void PlayCombo(int combo)
    {
        animator.SetInteger("Combo", combo);
    }

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