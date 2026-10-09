using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 敌人状态枚举：idle闲置 → walk巡逻 → pursuit追击 → attack攻击 → getHit受伤 → dead死亡
/// </summary>
public enum EnemyState { idle, walk, pursuit, attack, getHit, dead }

/// <summary>
/// 敌人基类：实现有限状态机（FSM），管理巡逻、追击、攻击、受伤、死亡全流程。
/// 子类（EnemyBear / EnemyLancer / EnemyTroll）可重写虚方法实现差异化行为。
/// 实现 IDamageable 接口以统一受伤逻辑。
/// </summary>
[RequireComponent(typeof(EnemyAttack))]
public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("组件")]
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public EnemyState state = EnemyState.walk;
    // 旧状态，在 ChangeState 时记录，供新状态的 Enter 方法判断来源
    public EnemyState stateOld = EnemyState.walk;
    public Slider hpSlider;
    protected EnemyAttack enemyAttack;

    private EnemyAnimation enemyAnimation;

    [Header("移动相关变量")]
    // 巡逻路径的两个端点
    public Transform pos1;
    public Transform pos2;
    // 当前移动目标（巡逻端点或玩家）
    public Transform targetPos;

    [Header("攻击相关")]
    private float getHitTimer = 0f;
    // 受击击退方向，由 TakeDamage 传入
    private Vector2 attackerDirection;
    // 敌人及其子对象的父级，死亡后整体销毁
    public GameObject enemyAndPos;
    // 玩家是否在警戒范围内（由 EnemypursuitCollider 维护）
    protected bool isPlayerInRange;

    [Header("基础属性")]
    public float ATK = 10f;
    public float HPMax = 100f;
    public float HPNow = 100f;

    public float speed = 1f;

    private void Awake()
    {
        enemyAnimation = GetComponent<EnemyAnimation>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    public virtual void Start()
    {
        ChangeState(EnemyState.walk);
        targetPos = pos1;
        HPNow = HPMax;
    }


    public virtual void Update()
    {
        // 根据当前状态分发到对应的 Update 方法
        switch (state)
        {
            case EnemyState.idle:
                idleUpdate();
                break;

            case EnemyState.walk:
                walkUpdate();
                break;

            case EnemyState.pursuit:
                pursuitUpdate();
                break;

            case EnemyState.attack:
                attackUpdate();
                break;

            case EnemyState.getHit:
                getHitUpdate();
                break;

            case EnemyState.dead:
                deadUpdate();
                break;
        }
    }

    public virtual void FixedUpdate()
    {

    }

    #region 状态机
    /// <summary>
    /// 状态切换：先调用旧状态的 Exit，再调用新状态的 Enter
    /// </summary>
    public virtual void ChangeState(EnemyState newState)
    {
        if (state == EnemyState.idle) { idleExit(); }
        else if (state == EnemyState.walk) { walkExit(); }
        else if (state == EnemyState.pursuit) { pursuitExit(); }
        else if (state == EnemyState.attack) { attackExit(); }
        else if (state == EnemyState.getHit) { getHitExit(); }
        else if (state == EnemyState.dead) { deadExit(); }
        stateOld = state;
        state = newState;
        if (state == EnemyState.idle) { idleEnter(); }
        else if (state == EnemyState.walk) { walkEnter(); }
        else if (state == EnemyState.pursuit) { pursuitEnter(); }
        else if (state == EnemyState.attack) { attackEnter(); }
        else if (state == EnemyState.getHit) { getHitEnter(); }
        else if (state == EnemyState.dead) { deadEnter(); }
    }

    // === idle 状态：原地停留，2秒后自动回到巡逻 ===
    public virtual void idleEnter()
    {
        enemyAnimation.SetMove(false);
        if (stateOld == EnemyState.walk)
        {
            CancelInvoke(nameof(IdleToWalk));
            Invoke(nameof(IdleToWalk), 2f);
        }
    }
    public virtual void idleUpdate()
    {
        rb.linearVelocity = Vector2.zero;
    }
    public virtual void idleExit()
    {

    }

    // === walk 状态：在 pos1 和 pos2 之间巡逻 ===
    public virtual void walkEnter()
    {
        enemyAnimation.SetMove(true);
    }
    public virtual void walkUpdate()
    {
        rb.linearVelocity = (targetPos.position - transform.position).normalized * speed;
        sr.flipX = rb.linearVelocity.x < 0;

        // 到达 pos1，切换目标为 pos2 并进入 idle
        if (Vector2.Distance(transform.position, pos1.position) < 0.1f)
        {
            if (targetPos == pos1)
            {
                targetPos = pos2;
                ChangeState(EnemyState.idle);
            }
        }
        // 到达 pos2，切换目标为 pos1 并进入 idle
        else if (Vector2.Distance(transform.position, pos2.position) < 0.1f)
        {
            if (targetPos == pos2)
            {
                targetPos = pos1;
                ChangeState(EnemyState.idle);
            }
        }
    }
    public virtual void walkExit()
    {

    }

    // === pursuit 状态：追击玩家，进入攻击距离且朝向正确则切到 attack ===
    public virtual void pursuitEnter()
    {
        enemyAnimation.SetMove(true);
    }
    public virtual void pursuitUpdate()
    {
        rb.linearVelocity = (targetPos.position - transform.position).normalized * speed;
        sr.flipX = rb.linearVelocity.x < 0;

        // 在攻击距离内且朝向目标 → 进入攻击
        if (Vector2.Distance(transform.position, targetPos.position) < enemyAttack.attackDistance)
        {
            if (sr.flipX && (targetPos.position.x - transform.position.x) < 0)
            {
                ChangeState(EnemyState.attack);
            }
            else if (!sr.flipX && (targetPos.position.x - transform.position.x) > 0)
            {
                ChangeState(EnemyState.attack);
            }
        }
    }
    public virtual void pursuitExit()
    {

    }

    // === attack 状态：委托给 EnemyAttack 处理 ===
    public virtual void attackEnter()
    {
        enemyAttack.EnterAttack();
    }
    public virtual void attackUpdate()
    {
        enemyAttack.UpdateAttack();
    }
    public virtual void attackExit()
    {
        enemyAttack.ExitAttack();
    }

    // === getHit 状态：播放受伤动画，短暂击退，1秒后恢复追击 ===
    public virtual void getHitEnter()
    {
        enemyAnimation.PlayHurt();
        enemyAnimation.SetMove(false);

        rb.linearVelocity = Vector2.zero;
        getHitTimer = 0f;
    }
    public virtual void getHitUpdate()
    {
        getHitTimer += Time.deltaTime;

        // 前0.2秒受击击退
        if (getHitTimer <= 0.2f)
        { rb.linearVelocity = attackerDirection * 2f; }
        else
        { rb.linearVelocity = Vector2.zero; }

        // 1秒后根据玩家是否在警戒范围内，决定回到追击还是巡逻
        // 远程技能（如火球）打中时玩家可能不在范围内，此时应继续巡逻而不是追击空气
        if (getHitTimer > 1f)
        {
            getHitTimer = 0f;
            if (isPlayerInRange)
            {
                ChangeState(EnemyState.pursuit);
            }
            else
            {
                ChangeState(EnemyState.walk);
            }
        }
    }
    public virtual void getHitExit()
    {

    }

    // === dead 状态：播放死亡动画，延迟销毁 ===
    public virtual void deadEnter()
    {
        enemyAnimation.SetMove(false);
        enemyAnimation.PlayDead();

        Invoke(nameof(DestroyEnemyAndPos), 1f);
        Destroy(hpSlider.transform.parent.gameObject); // 销毁怪物身上的血条UI
    }
    public virtual void deadUpdate()
    {
        rb.linearVelocity = Vector2.zero;
    }
    public virtual void deadExit()
    {

    }

    // === 状态间转换的辅助方法 ===
    public virtual void IdleToWalk()
    {
        ChangeState(EnemyState.walk);
    }
    public virtual void AttackToWalk()
    {
        targetPos = pos1;
        ChangeState(EnemyState.walk);
    }

    public virtual void DestroyEnemyAndPos()
    {
        Destroy(enemyAndPos);
    }
    #endregion

    /// <summary>
    /// 玩家进入警戒范围，由 EnemypursuitCollider 触发
    /// </summary>
    public virtual void PlayerEnterPursuitBox(Transform target)
    {
        if (state == EnemyState.dead)
        {
            return;
        }
        isPlayerInRange = true;
        CancelInvoke(nameof(IdleToWalk));
        targetPos = target;
        ChangeState(EnemyState.pursuit);
    }

    /// <summary>
    /// 玩家退出警戒范围，由 EnemypursuitCollider 触发
    /// </summary>
    public virtual void PlayerExitPursuitBox()
    {
        isPlayerInRange = false;
        if (state == EnemyState.attack)
        {
            // 等当前攻击冷却结束后再回到巡逻
            Invoke(nameof(AttackToWalk), enemyAttack.attackCooldown - enemyAttack.AttackTimer);
        }
        else if (state == EnemyState.dead)
        {
            return;
        }
        else
        {
            targetPos = pos1;
            ChangeState(EnemyState.walk);
        }

    }

    /// <summary>
    /// IDamageable 实现：扣血、更新血条、进入受击或死亡状态
    /// </summary>
    public virtual void TakeDamage(float damage, Vector2 attackDirection)
    {
        if (HPNow <= 0) { return; }

        HPNow -= damage;
        hpSlider.value = HPNow / HPMax;
        attackerDirection = attackDirection;

        if (HPNow <= 0)
        {
            ChangeState(EnemyState.dead);
        }
        else
        {
            ChangeState(EnemyState.getHit);
        }
    }

    public Transform TargetPos
    {
        get
        {
            return targetPos;
        }
    }


    #region ===== Property =====

    public Rigidbody2D Rigidbody => rb;

    public EnemyAnimation Animation => enemyAnimation;

    public SpriteRenderer SpriteRenderer => sr;

    public Transform Target => targetPos;

    public float AttackPower => ATK;

    #endregion
}
