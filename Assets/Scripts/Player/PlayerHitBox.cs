using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家攻击碰撞盒
/// 只负责检测敌人进入攻击范围
/// 不负责播放动画，也不负责攻击逻辑
/// </summary>
public class PlayerHitBox : MonoBehaviour
{
    // =========================
    // Inspector 可调参数
    // =========================

    [Header("攻击配置")]
    public int attackDamage = 10;

    // =========================
    // 私有变量
    // =========================

    /// <summary>
    /// 当前HitBox是否开启
    /// false时，即使敌人进入也不会造成伤害
    /// </summary>
    private bool isActive = false;

    /// <summary>
    /// 记录这一刀已经命中过的敌人
    /// HashSet不会存储重复元素，因此非常适合做去重
    /// </summary>
    private HashSet<EnemyBase> hitEnemies = new HashSet<EnemyBase>();

    // 后面我们会继续补充 OnTriggerEnter、EnableHitBox、DisableHitBox 等函数
}