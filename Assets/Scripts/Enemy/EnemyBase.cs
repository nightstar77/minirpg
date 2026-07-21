using UnityEngine;
using UnityEngine.UI;

public enum EnemyState { idle, walk, pursuit, attack, getHit, dead }
//public enum EnemyState { Idle, Patrol, Chase, Attack, Hurt, Dead }

[RequireComponent(typeof(EnemyAttack))]
public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("组件")]
    public Rigidbody2D rb;
    //public Animator am;
    public SpriteRenderer sr;
    public EnemyState state = EnemyState.walk;
    public EnemyState stateOld = EnemyState.walk;
    public Slider hpSlider;
    protected EnemyAttack enemyAttack;

    private EnemyAnimation enemyAnimation;

    [Header("移动相关变量")]
    public Transform pos1;
    public Transform pos2;
    public Transform targetPos;

    [Header("攻击相关")]
    private float getHitTimer = 0f;
    private Vector2 attackerDirection;
    public GameObject enemyAndPos;

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

    #region ״̬状态机
    /// <summary>
    /// �л�Ϊ��״̬
    /// </summary>
    /// <param name="newState">��״̬</param>
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

    public virtual void walkEnter()
    {
        enemyAnimation.SetMove(true);
    }
    public virtual void walkUpdate()
    {
        rb.linearVelocity = (targetPos.position - transform.position).normalized * speed;
        sr.flipX = rb.linearVelocity.x < 0;

        if (Vector2.Distance(transform.position, pos1.position) < 0.1f)//����pos1
        {
            if (targetPos == pos1)                       //��ɫĿ���pos1
            {
                targetPos = pos2;                       //��ɫĿ���仯pos2
                ChangeState(EnemyState.idle);           //��ɫ״̬�л�Ϊidle
            }
        }
        else if (Vector2.Distance(transform.position, pos2.position) < 0.1f)//����pos2
        {
            if (targetPos == pos2)                       //��ɫĿ���pos2
            {
                targetPos = pos1;                       //��ɫĿ���仯pos1
                ChangeState(EnemyState.idle);           //��ɫ״̬�л�Ϊidle
            }
        }
    }
    public virtual void walkExit()
    {

    }

    public virtual void pursuitEnter()
    {
        enemyAnimation.SetMove(true);
    }
    public virtual void pursuitUpdate()
    {
        rb.linearVelocity = (targetPos.position - transform.position).normalized * speed;
        sr.flipX = rb.linearVelocity.x < 0;

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

        //受到攻击后退
        if (getHitTimer <= 0.2f)
        { rb.linearVelocity = attackerDirection * -2f; }
        else
        { rb.linearVelocity = Vector2.zero; }

        //受到攻击一段时间后回到pursuit状态
        if (getHitTimer > 1f)
        {
            getHitTimer = 0f;
            ChangeState(EnemyState.pursuit);
        }
    }
    public virtual void getHitExit()
    {

    }

    public virtual void deadEnter()
    {
        enemyAnimation.SetMove(false);
        enemyAnimation.PlayDead();

        Invoke(nameof(DestroyEnemyAndPos), 1f);
        Destroy(hpSlider.transform.parent.gameObject);//销毁怪物身上的UI
    }
    public virtual void deadUpdate()
    {
        rb.linearVelocity = Vector2.zero;
    }
    public virtual void deadExit()
    {

    }

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
    /// 玩家进入警戒范围
    /// </summary>
    /// <param name="player">玩家</param>
    public virtual void PlayerEnterPursuitBox(Transform target)
    {
        if (state == EnemyState.dead)
        {
            return;
        }
        CancelInvoke(nameof(IdleToWalk));
        targetPos = target;
        ChangeState(EnemyState.pursuit);
    }

    /// <summary>
    /// 玩家退出警戒范围
    /// </summary>
    /// <param name="player">玩家</param>
    public virtual void PlayerExitPursuitBox()
    {
        if (state == EnemyState.attack)
        {
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
    /// 受到伤害
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(float damage, Vector2 attackDirection)
    {
        if (HPNow <= 0) { return; }

        HPNow -= damage;//受到伤害
        hpSlider.value = HPNow / HPMax;//血条变化
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
