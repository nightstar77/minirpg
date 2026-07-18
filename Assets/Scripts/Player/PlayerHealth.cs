using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
    #region ===== Inspector =====
    [Header("生命设置")]
    public float maxHP = 100;
    [Header("击退设置")]
    public float knockBackForce = 8f;

    #endregion

    #region ===== Runtime =====
    private float currentHP;
    private bool isInvincible;                                          // 是否处于无敌时间
    private float invincibleTimer;                                      // 无敌时间计时器
    public float invincibleDuration = 0.8f;                             // 无敌持续时间

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
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10, GameObject.Find("Enemy-Bear").transform);
        }
        UpdateInvincible();
    }

    /// <summary>
    /// 玩家受到伤害
    /// </summary>
    public void TakeDamage(float damage, Transform attacker)
    {
        //死亡后不能继续受伤
        if (playerState.IsState(PlayerState.State.Dead))
        {
            return;
        }
        if (isInvincible)
        {
            Debug.Log("无敌状态");
            return;
        }

        currentHP -= damage;

        Debug.Log("玩家受到伤害:" + damage);
        StartInvincible();
        //取消当前攻击
        playerAttack.CancelAttack();
        if (currentHP <= 0)
        {
            Dead();
        }
        else
        {
            Hurt(attacker);
            ApplyKnockBack(attacker);
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

    private void Hurt(Transform attacker)
    {
        playerState.ChangeState(PlayerState.State.Hurt);
        playerAnimation.PlayHurt();
        Invoke(nameof(RecoverFromHurt), 0.5f);
    }

    private void RecoverFromHurt()
    {
        if (playerState.IsState(PlayerState.State.Hurt))
        {
            playerState.ChangeState(PlayerState.State.Idle);
        }
    }

    private void ApplyKnockBack(Transform attacker)
    {
        if (attacker == null)
        {
            Debug.Log("攻击者为空");
            return;
        }

        Vector2 direction = (transform.position - attacker.position).normalized;
        Debug.Log("击退方向：" + direction);
        rb.AddForce(direction * knockBackForce, ForceMode2D.Impulse);
    }

    private void Dead()
    {
        playerState.ChangeState(PlayerState.State.Dead);
        playerAnimation.PlayDead();
    }

}