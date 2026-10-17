using UnityEngine;

/// <summary>
/// 掉落金币：从金矿弹出、滑行减速，玩家靠近后吸附飞向玩家并计入金币。
/// 生成流程：弹出（物理滑行）→ 停住 → 玩家进入拾取范围 → 吸附 → 拾取销毁。
///
/// 预制体配置：SpriteRenderer + Rigidbody2D（Gravity Scale = 0）
/// + CircleCollider2D（不勾 IsTrigger，用于撞墙/障碍时停下）
/// </summary>
public class GoldCoin : MonoBehaviour
{
    #region ===== Inspector =====
    [Header("金币设置")]
    [Tooltip("单枚金币价值")]
    [SerializeField]
    private int value = 1;

    [Tooltip("弹出初速度")]
    [SerializeField]
    private float popSpeed = 2.5f;

    [Tooltip("生成后多久才可被吸附（给弹出散开留时间）")]
    [SerializeField]
    private float pickupDelay = 0.4f;

    [Tooltip("玩家进入该距离后金币开始吸附")]
    [SerializeField]
    private float pickupRadius = 1.5f;

    [Tooltip("吸附飞行速度")]
    [SerializeField]
    private float flySpeed = 8f;

    [Tooltip("距玩家多近算拾取成功")]
    [SerializeField]
    private float pickupDistance = 0.3f;
    #endregion

    #region ===== Component =====
    private Rigidbody2D rb;
    private PlayerGold playerGold;   // 拾取目标玩家的金币组件（懒加载缓存）
    #endregion

    #region ===== Runtime =====
    private bool isFlying;           // 是否正在吸附飞向玩家
    private float spawnTime;
    #endregion

    #region ===== Unity =====
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnTime = Time.time;
        // 随机方向弹出，让多枚金币散开
        Vector2 dir = Random.insideUnitCircle.normalized;
        rb.linearVelocity = dir * popSpeed;
    }

    private void Update()
    {
        if (isFlying)
        {
            FlyToPlayer();
            return;
        }
        // 生成初期不吸附，先让金币弹出散开
        if (Time.time - spawnTime < pickupDelay)
        {
            return;
        }
        TryStartFlying();
    }
    #endregion

    #region ===== Pickup =====
    /// <summary>
    /// 检测玩家距离，进入拾取范围则切换为吸附状态
    /// </summary>
    private void TryStartFlying()
    {
        if (playerGold == null)
        {
            // 场景中只有一个玩家，找到后缓存
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj == null)
            {
                return;   // 玩家不存在（死亡等），金币停在原地
            }
            playerGold = playerObj.GetComponent<PlayerGold>();
            if (playerGold == null)
            {
                Debug.LogWarning("玩家身上没有 PlayerGold 组件，金币无法被拾取");
                return;
            }
        }

        float dist = Vector2.Distance(transform.position, playerGold.transform.position);
        if (dist <= pickupRadius)
        {
            isFlying = true;
            // 切运动学，吸附飞行不再受物理影响
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    /// <summary>
    /// 吸附飞行：每帧朝玩家移动，足够近即拾取
    /// </summary>
    private void FlyToPlayer()
    {
        if (playerGold == null)
        {
            Destroy(gameObject);   // 玩家没了（切场景/死亡销毁），金币自毁避免悬空
            return;
        }
        Vector2 targetPos = playerGold.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, flySpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, targetPos) <= pickupDistance)
        {
            Pickup();
        }
    }

    /// <summary>
    /// 拾取：计入玩家金币并销毁
    /// </summary>
    private void Pickup()
    {
        playerGold.AddGold(value);
        Debug.Log("拾取金币 +" + value);
        Destroy(gameObject);
    }
    #endregion
}
