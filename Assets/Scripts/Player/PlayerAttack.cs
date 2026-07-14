using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private PlayerAnimation playerAnimation;

    public int attackDamage = 10;
    public float attackRange = 1.5f;

    public LayerMask enemylayer;
    void Start()
    {
        animator = GetComponent<Animator>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }

    }

    void TryAttack()
    {
        playerAnimation.PlayAttack();
        DetectAttack();
    }

    void DetectAttack()
    {
        /*
         * Physics.OverlapSphere()
         * Unity物理检测API
         * 作用：检测一个球形范围内所有Collider
         * 参数1：中心位置
         * 参数2：半径
         * 参数3：检测Layer
         */
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position,attackRange,enemylayer);

        /*
         * 遍历所有检测到的敌人
         */
        foreach (Collider enemy in hitEnemies)
        {
            EnemyBase enemyScript;

            if (enemy.TryGetComponent<EnemyBase>(out enemyScript))
            {
                /*
                 * 调用敌人的受伤函数
                 */
                //enemyScript.TakeDamage(attackDamage, transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        /*
         *Gizmos
         *Unity调试绘制工具
         */
        Gizmos.DrawWireSphere(transform.position,attackRange);


    }


}
