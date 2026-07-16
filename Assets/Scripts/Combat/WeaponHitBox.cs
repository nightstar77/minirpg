using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 武器攻击盒。
/// 负责检测敌人并造成伤害。
/// 不负责播放动画，不负责响应输入。
/// </summary>
public class WeaponHitBox : MonoBehaviour
{
    // ===========================
    // Inspector 参数
    // ===========================

    [Header("攻击属性")]
    // 每次攻击造成的伤害
    public int damage = 10;

    // ===========================
    // Unity 回调函数
    // ===========================

    /// <summary>
    /// 当其它 Trigger / Collider2D 进入本碰撞盒时自动调用。
    /// 注意：不需要我们手动调用，Unity 物理系统会自动触发。
    /// </summary>
    /// <param name="other">
    /// 与当前 HitBox 发生接触的 Collider2D。
    /// </param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 判断进入碰撞盒的物体是否属于 Enemy 标签
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        // 尝试获取 Enemy 脚本
        EnemyBase enemy = other.GetComponent<EnemyBase>();

        // 如果成功获取到 Enemy，则造成伤害
        if (enemy != null)
        {
            enemy.TakeDamage(damage,transform);
        }
    }
}