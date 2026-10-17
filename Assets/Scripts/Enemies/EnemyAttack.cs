using UnityEngine;

/// <summary>
/// 敌人攻击控制器：管理攻击冷却、距离判定与攻击预制体生成。
/// 由 EnemyBase 的 attack 状态调用，不自行驱动状态切换以外的逻辑。
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    [Header("组件")]
    private EnemyBase enemy;
    private SpriteRenderer spriteRenderer;

    [Header("攻击相关")]
    public float attackDistance = 1f;
    public float attackCooldown = 1f;
    public GameObject attackPrefab;
    public Transform attackPointLeft;
    public Transform attackPointRight;


    #region Runtime

    private float attackTimer;
    private bool canAttack = true;

    #endregion

    private void Awake()
    {
        enemy = GetComponent<EnemyBase>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 进入攻击状态时调用，停止移动动画
    /// </summary>
    public void EnterAttack()
    {
        enemy.Animation.SetMove(false);
    }

    /// <summary>
    /// 攻击状态每帧逻辑：判断距离与朝向，满足条件时播放攻击动画并进入冷却
    /// </summary>
    public void UpdateAttack()
    {
        enemy.Rigidbody.linearVelocity = Vector2.zero;
        if (canAttack)
        {
            // 超出攻击距离 → 切回追击
            if (Vector2.Distance(transform.position, enemy.Target.position) > attackDistance)
            {
                enemy.ChangeState(EnemyState.pursuit);
            }
            // 朝向与目标不一致 → 切回追击调整方向
            else if (enemy.SpriteRenderer.flipX && (enemy.Target.position.x - transform.position.x) > 0)
            {
                enemy.ChangeState(EnemyState.pursuit);
            }
            else if (!enemy.SpriteRenderer.flipX && (enemy.Target.position.x - transform.position.x) < 0)
            {
                enemy.ChangeState(EnemyState.pursuit);
            }
            // 在攻击距离内且朝向正确 → 执行攻击
            else
            {
                enemy.Animation.PlayAttack();
                canAttack = false;
            }
        }

        // 攻击冷却计时
        if (attackTimer < attackCooldown)
        {
            attackTimer += Time.deltaTime;
        }
        else
        {
            canAttack = true;
            attackTimer = 0f;
        }
    }

    /// <summary>
    /// 离开攻击状态时重置冷却
    /// </summary>
    public void ExitAttack()
    {
        attackTimer = 0f;
        canAttack = true;
    }

    /// <summary>
    /// 动画事件回调：根据朝向在对应位置生成攻击预制体
    /// </summary>
    public void Attack1()
    {
        GameObject go;

        if (spriteRenderer.flipX)
        {
            go = Instantiate(attackPrefab, attackPointLeft.position, attackPointLeft.rotation);
            go.transform.localScale = attackPointLeft.localScale;
        }
        else
        {
            go = Instantiate(attackPrefab, attackPointRight.position, attackPointRight.rotation);
            go.transform.localScale = attackPointRight.localScale;
        }
        Vector2 direction = (enemy.TargetPos.position - transform.position).normalized;
        go.GetComponent<AttackPrefeb>().Init(false, enemy.AttackPower, direction);
    }


    public float AttackTimer => attackTimer;
}
