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
        if (weapon != null)
        {
            weapon.Init(transform);
        }
        else
        {
            Debug.LogError("PlayerAttack没有绑定WeaponHitBox");
        }
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

    #endregion

    #region ===== Property =====

    /// <summary>
    /// 当前是否正在攻击
    /// 提供给其它脚本读取
    /// </summary>
    public bool IsAttacking => isAttacking;

    #endregion
}