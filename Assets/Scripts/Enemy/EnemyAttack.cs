using UnityEngine;

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

    public void EnterAttack()
    {
        enemy.Animation.SetMove(false);
    }

    public void UpdateAttack()
    {
        enemy.Rigidbody.linearVelocity = Vector2.zero;
        if (canAttack)
        {
            if (Vector2.Distance(transform.position, enemy.Target.position) > attackDistance)//在攻击距离外
            {
                enemy.ChangeState(EnemyState.pursuit);
            }
            else if (enemy.SpriteRenderer.flipX && (enemy.Target.position.x - transform.position.x) > 0)//在攻击距离内但在反方向
            {
                enemy.ChangeState(EnemyState.pursuit);
            }
            else if (!enemy.SpriteRenderer.flipX && (enemy.Target.position.x - transform.position.x) < 0)//在攻击距离内但在反方向
            {
                enemy.ChangeState(EnemyState.pursuit);
            }
            else//在攻击距离内且方向正确
            {
                enemy.Animation.PlayAttack();
                canAttack = false;
            }
        }

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

    public void ExitAttack()
    {
        attackTimer = 0f;
        canAttack = true;
    }
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
