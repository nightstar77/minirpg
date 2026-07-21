using UnityEngine;
using System.Collections.Generic;

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
    private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
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
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        { return; }
        IDamageable target = other.GetComponentInParent<IDamageable>();
        if (target == null)
        {
            return;
        }
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
        Vector2 attackDirection = (other.transform.position - attacker.position).normalized;
        target.TakeDamage(damage, attackDirection);
        Debug.Log("攻击检测到：" + other.name);
    }
    #endregion

    #region ===== Public =====
    public void Init(Transform owner)
    {
        attacker = owner;
    }
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