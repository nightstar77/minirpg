using UnityEngine;

/// <summary>
/// 金矿：可被玩家普攻破坏的资源点，碎裂后掉落金币。
/// 实现 IDamageable 接口，被 WeaponHitBox 命中时扣减耐久。
///
/// 大小金矿用法：同一个脚本挂不同预制体，Inspector 配不同参数即可——
/// 大金矿调大耐久（maxDurability）和金币数（coinCount），小金矿调小。
///
/// 闪白动画：受击时通过 oreAnimator 触发 "Hit" trigger，碎裂时触发 "Break" trigger，
/// 动画由你自己在 Animator Controller 中制作（参数名可在 Inspector 修改）。
/// </summary>
public class GoldOre : MonoBehaviour, IDamageable
{
    #region ===== Inspector =====
    [Header("金矿设置")]
    [Tooltip("耐久：被打多少次后碎裂")]
    [SerializeField]
    private int maxDurability = 3;

    [Tooltip("碎裂后掉落的金币数量（大金矿调大，小金矿调小）")]
    [SerializeField]
    private int coinCount = 3;

    [Tooltip("掉落的金币预制体（需挂 GoldCoin 脚本）")]
    [SerializeField]
    private GoldCoin coinPrefab;

    [Header("动画（可选）")]
    [Tooltip("金矿的 Animator，受击/碎裂动画触发用；不接就只掉金币")]
    [SerializeField]
    private Animator oreAnimator;

    [Tooltip("受击动画的 Trigger 参数名")]
    [SerializeField]
    private string hitTriggerName = "Hit";

    [Tooltip("碎裂动画的 Trigger 参数名")]
    [SerializeField]
    private string breakTriggerName = "Break";

    [Tooltip("碎裂动画播放多久后销毁金矿本体")]
    [SerializeField]
    private float breakDestroyDelay = 0.6f;
    #endregion

    #region ===== Runtime =====
    private int currentDurability;
    private bool isBroken;   // 防止碎裂后重复结算
    #endregion

    #region ===== Unity =====
    private void Awake()
    {
        currentDurability = maxDurability;
    }
    #endregion

    #region ===== IDamageable =====
    /// <summary>
    /// 被玩家武器命中时扣减耐久。
    /// 金矿没有击退和受击硬直，attackDirection 参数忽略。
    /// </summary>
    public void TakeDamage(float damage, Vector2 attackDirection)
    {
        if (isBroken)
        {
            return;
        }
        currentDurability--;

        // 触发受击闪白动画（Animator 未接时跳过）
        if (oreAnimator != null)
        {
            oreAnimator.SetTrigger(hitTriggerName);
        }

        if (currentDurability <= 0)
        {
            Break();
        }
    }
    #endregion

    #region ===== Break =====
    /// <summary>
    /// 矿石碎裂：关闭碰撞、触发碎裂动画、掉落金币、延迟销毁本体
    /// </summary>
    private void Break()
    {
        isBroken = true;

        // 关闭自身碰撞体，避免碎裂动画期间还能被打或挡路
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        if (oreAnimator != null)
        {
            oreAnimator.SetTrigger(breakTriggerName);
        }

        // 掉落金币：位置加一点随机偏移，散开更自然
        for (int i = 0; i < coinCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 0.3f;
            Instantiate(coinPrefab, transform.position + (Vector3)offset, Quaternion.identity);
        }

        Debug.Log(name + " 被打碎，掉落金币 x" + coinCount);
        Destroy(gameObject, breakDestroyDelay);
    }
    #endregion
}
