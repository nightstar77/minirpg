using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("组件")]
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;
    public Slider hpSlider;
    public Text hpText;
    public GameObject deadUI;
    public AudioSource audioS;

    [Header("移动")]
    public float speed = 1.0f;

    [Header("攻击")]
    public GameObject attackPrefeb;
    public Transform attack1Pos;
    public AudioClip attackAudio;
    public AudioClip guardAudio;

    [Header("基础属性")]
    public float ATK = 10f;
    public float HPMax = 100f;
    public float HPNow = 100f;

    private Vector2 moveDirection;
    private int AttackCombo = 1;
    private bool isAttacking = false;
    private bool isGuard = false;
    private bool isDead = false;

    void Start()
    {
        Application.targetFrameRate = 60;
        HPNow = HPMax;
    }

    void Update()
    {
        PlayerWindowsInput();
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            PlayerMove();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    /// <summary>
    /// 输入
    /// </summary>
    public void PlayerWindowsInput()
    {
        //移动输入
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");

        //攻击输入
         if (Input.GetKeyDown(KeyCode.J))
        {
            PlayerAttack();
        }

        //防御输入
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerGuard(true);
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            PlayerGuard(false);
        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    public void PlayerMove()
    {
        if (!isAttacking && !isGuard)
        {
            rb.linearVelocity = moveDirection * speed;             //移动
            anim.SetBool("IsRun", moveDirection.magnitude > 0.1f); //播放动画
            if (moveDirection.magnitude > 0.1f)
            {
                sr.flipX = (moveDirection.x < -0.1f);                    //角色朝向
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;                     //攻击时停止移动
        }
        
    }

    /// <summary>
    /// 攻击动画
    /// </summary>
    public void PlayerAttack()
    {
        if (!isAttacking && !isGuard)//防止连续攻击
        {
            isAttacking = true;
            if (AttackCombo == 1)
            {
                anim.SetTrigger("Attack1");
                AttackCombo = 2;
            }
            else if (AttackCombo == 2)
            {
                anim.SetTrigger("Attack2");
                AttackCombo = 1;
            }
            Invoke(nameof(AttackEnd), 0.4f);
        }
    }

    public void AttackEnd()
    {
        isAttacking = false;
    }

    /// <summary>
    /// 防御姿态
    /// </summary>
    /// <param name="enterGuard">进入或退出防御状态</param>
    public void PlayerGuard(bool enterGuard)
    {
        if (!isDead)
        {
            anim.SetBool("IsGuard", enterGuard);
            isGuard = enterGuard;
        }
    }

    #region 动画事件
    public void Attack1()
    {
        GameObject go = Instantiate(attackPrefeb, attack1Pos.position, attack1Pos.rotation);
        go.transform.localScale = attack1Pos.localScale;
        go.GetComponent<AttackPrefeb>().Init(true, ATK, transform);
        audioS.PlayOneShot(attackAudio);
    }
    #endregion

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage">造成伤害的数值</param>
    /// <param name="attackPosition">攻击者的位置</param>
    public void TakeDamage(float damage, Transform attackPosition)
    {
        if(HPNow <= 0) { return; }
        if (isGuard)
        {
            if(transform.position.x < attackPosition.position.x && !sr.flipX)
            {
                audioS.PlayOneShot(guardAudio);//播放防御音效
                return;
            }
            else if(transform.position.x > attackPosition.position.x && sr.flipX)
            {
                audioS.PlayOneShot(guardAudio);
                return;
            }
        }

        HPNow -= damage;//受到伤害
        hpSlider.value = HPNow / HPMax;//血条
        hpText.text = HPNow.ToString("f0") + " / " + HPMax.ToString("f0");//血量显示

        if (HPNow <= 0) 
        {
            PlayerDead();
            return; 
        }
        else
        {
            anim.SetTrigger("GetHit");
            anim.SetBool("IsRun", false);
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void PlayerDead()
    {
        if (!isDead)
        {
            anim.SetTrigger("Dead");
            anim.SetBool("IsRun", false);
            rb.linearVelocity = Vector2.zero;
            isDead = true;
            deadUI.SetActive(true);
        }
        
    }
}
