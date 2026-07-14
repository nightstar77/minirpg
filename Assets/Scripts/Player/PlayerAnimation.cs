using UnityEngine;


// 玩家动画控制脚本
public class PlayerAnimation : MonoBehaviour
{
    // 保存Animator组件引用
    private Animator animator;

    // 保存玩家移动速度
    private float speed;
    private float attackflag = 0;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat("Speed",speed);
    }

    public void PlayAttack()
    {
        if(attackflag == 0)
        {
            animator.SetTrigger("Attack");
            attackflag = 1;
        }
        else if(attackflag == 1)
        {
            attackflag = 0;
            animator.SetTrigger("Attack1");
        }
    }

    public void PlayHurt()
    {
        animator.SetTrigger("Hurt");
    }

    public void PlayDead()
    {
        animator.SetTrigger("Dead");
    }

}
