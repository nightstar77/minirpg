using UnityEngine;
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
    private bool isAttacking;
    private int comboIndex;
    private bool canCombo;
    private bool attackQueued;

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
        if (Input.GetMouseButtonDown(0))
        {
            HandleAttackInput();
        }
        if (Input.GetMouseButtonDown(1))
        {
            CancelAttack();
        }
    }
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
    private void StartComboAttack()
    {
        isAttacking = true;
        playerState.ChangeState(PlayerState.State.Attack);
        rb.linearVelocity = Vector2.zero;
        comboIndex++;
        if (comboIndex > maxCombo)
        {
            comboIndex = 1;
        }
        playerAnimation.PlayCombo(comboIndex);
    }

    public void EnableCombo()
    {
        canCombo = true;
    }

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

    /// <summary>
    /// 强制取消当前攻击
    ///
    /// 使用场景：
    ///
    /// 1. 玩家受伤
    /// 2. 玩家死亡
    /// 3. 技能打断
    /// </summary>
    public void CancelAttack()
    {
        //关闭HitBox
        if (weapon != null)
        {
            weapon.DisableHitBox();
        }
        //取消攻击状态
        isAttacking = false;
        //取消连击
        comboIndex = 0;
        attackQueued = false;
        canCombo = false;
        //重置Animator参数
        playerAnimation.ResetCombo();
        //恢复玩家状态
        playerState.ChangeState(PlayerState.State.Idle);
        Debug.Log("攻击被取消");
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