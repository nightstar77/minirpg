using UnityEngine;

/// <summary>
/// 可受伤接口：统一玩家与敌人的受伤逻辑入口。
/// attackDirection 用于击退方向计算。
/// </summary>
public interface IDamageable
{
    void TakeDamage(float damage, Vector2 attackDirection);
}