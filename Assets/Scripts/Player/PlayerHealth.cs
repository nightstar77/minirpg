using UnityEngine;
using System;

/// <summary>
/// 玩家生命系统：管理血量、无敌帧、击退、防御、死亡。
/// 实现 IDamageable 接口，被攻击时由攻击方调用 TakeDamage。
/// </summary>
public class PlayerHealth : MonoBehaviour, IDamageable
{
    #region ===== Inspector =====
    [Header("生命设置")]
    public float maxHP = 100;
    [Header("击退设置")]
    public float knockBackForce = 1f;

    #endregion

    #region ===== Runtime =====
    private float currentHP;
    private bool isInvincible;                  // 是否处于无敌时间
    private float invincibleTimer;              // 无敌时间计时器
    public float invincibleDuration = 0.8f;     // 无敌持续时间
    public event Action OnPlayerDead;           // 玩家死亡事件，UI 等系统订阅
    private bool isDefending;                   // 防御中，免疫伤害

    #endregion

    #region ===== Component =====
    private Rigidbody2D rb;
    private PlayerState playerState;
    private PlayerAnimation playerAnimation;
    private PlayerAttack playerAttack;

    #endregion


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerState = GetComponent<PlayerState>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    private void Start()
    {
        currentHP = maxHP;
    }

    void Update()
    {
        UpdateInvincible();
    }

    /// <summary>
    /// IDamageable 实现：处理受伤、无敌帧、击退、死亡判定
    /// </summary>
    public void TakeDamage(float damage, Vector2 attackDirection)
    {
        // 死亡后不再受伤
        if (playerState.IsState(PlayerState.State.Dead))
        {
            return;
        }
        // 无敌帧内免疫伤害
        if (isInvincible)
        {
            Debug.Log("无敌状态");
            return;
        }
        // 防御中免疫伤害
        if (isDefending)
        {
            Debug.Log("防御成功，抵挡伤害");
            return;
        }
        currentHP -= damage;

        StartInvincible();
        // 受伤时强制取消当前攻击
        playerAttack.CancelAttack();
        if (currentHP <= 0)
        {
            Dead();
        }
        else
        {
            Hurt();
            ApplyKnockBack(attackDirection);
        }
    }

    private void StartInvincible()
    {
        isInvincible = true;
        invincibleTimer = 0;
        Debug.Log("进入无敌状态");
    }

    private void UpdateInvincible()
    {
        if (!isInvincible)
        {
            return;
        }
        invincibleTimer += Time.deltaTime;
        if (invincibleTimer >= invincibleDuration)
        {
            isInvincible = false;
            invincibleTimer = 0;
            Debug.Log("无敌结束");
        }
    }

    private void Hurt()
    {
        playerState.ChangeState(PlayerState.State.Hurt);
        playerAnimation.PlayHurt();
        // 0.5秒后从受伤状态恢复
        Invoke(nameof(RecoverFromHurt), 0.5f);
    }

    private void RecoverFromHurt()
    {
        if (playerState.IsState(PlayerState.State.Hurt))
        {
            playerState.ChangeState(PlayerState.State.Idle);
        }
    }

    private void ApplyKnockBack(Vector2 direction)
    {
        rb.AddForce(direction.normalized * knockBackForce, ForceMode2D.Impulse);
    }

    private void Dead()
    {
        playerState.ChangeState(PlayerState.State.Dead);
        playerAnimation.PlayDead();

        OnPlayerDead?.Invoke();
    }

    /// <summary>
    /// 开始防御，由 DefenseSkill 调用
    /// </summary>
    public void StartDefend()
    {
        isDefending = true;
        Debug.Log("进入防御状态");
    }

    /// <summary>
    /// 结束防御，由 DefenseSkill 调用
    /// </summary>
    public void EndDefend()
    {
        isDefending = false;
        Debug.Log("结束防御状态");
    }

    public float CurrentHP
    {
        get { return currentHP; }
    }
}