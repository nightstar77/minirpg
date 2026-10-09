using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 武器碰撞盒：挂在玩家武器子对象上，控制攻击判定的开启/关闭与伤害触发。
/// 通过 HashSet 记录本次挥击已命中目标，避免同一攻击多次伤害同一敌人。
/// </summary>
public class WeaponHitBox : MonoBehaviour
{
    #region ===== Inspector =====
    [Header("攻击设置")]
    public float damage = 10;
    #endregion

    #region ===== Component =====
    private Collider2D hitBox;
    #endregion

    #region Runtime
    // 本次挥击已命中的目标集合，每次开启 HitBox 前需清空
    private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
    // 攻击者 Transform，用于计算攻击方向（击退）
    private Transform attacker;
    #endregion

    #region ===== Unity =====
    private void Awake()
    {
        hitBox = GetComponent<Collider2D>();
        DisableHitBox();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 仅检测 Enemy 层
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        { return; }
        IDamageable target = other.GetComponentInParent<IDamageable>();
        if (target == null)
        {
            return;
        }
        // 防止同一攻击重复命中同一目标
        if (hitTargets.Contains(target))
        {
            return;
        }
        hitTargets.Add(target);
        if (attacker == null)
        {
            Debug.LogWarning("WeaponHitBox没有初始化攻击者");
            return;
        }
        // 计算从攻击者到被击者的方向，用于击退
        Vector2 attackDirection = (other.transform.position - attacker.position).normalized;
        target.TakeDamage(damage, attackDirection);
        Debug.Log("攻击检测到：" + other.name);
    }
    #endregion

    #region ===== Public =====
    /// <summary>
    /// 初始化攻击者引用，在 PlayerAttack.Awake 中调用
    /// </summary>
    public void Init(Transform owner)
    {
        attacker = owner;
    }

    /// <summary>
    /// 清空已命中目标列表，每次新挥击开始前调用
    /// </summary>
    public void ResetHitTargets()
    {
        hitTargets.Clear();
        Debug.Log("HitBox伤害列表重置");
    }

    public void EnableHitBox()
    {
        hitBox.enabled = true;
        Debug.Log("HitBox 开启");
    }

    public void DisableHitBox()
    {
        hitBox.enabled = false;
        Debug.Log("HitBox关闭");
    }
    #endregion
}