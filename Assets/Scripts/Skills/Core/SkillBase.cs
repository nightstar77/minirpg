using UnityEngine;
using System;

/// <summary>
/// 技能基类：所有具体技能（Dash/Fireball/Explosion）继承此类。
/// 提供冷却计时、事件通知和可用性检查的通用框架。
/// 子类只需实现 Data 属性和 Use() 方法。
/// </summary>
public abstract class SkillBase : MonoBehaviour
{
    protected abstract SkillData Data { get; }
    public SkillType SkillType => Data.skillType;

    // 技能使用事件，UI 订阅后显示冷却动画
    public event Action OnSkillUsed;
    // 冷却结束事件，UI 订阅后隐藏冷却遮罩
    public event Action OnCooldownFinished;

    protected float cooldownTimer;

    protected virtual void Update()
    {
        UpdateCooldown();
    }

    private void UpdateCooldown()
    {
        if (cooldownTimer <= 0)
        {
            return;
        }
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0)
        {
            cooldownTimer = 0;
            OnCooldownFinished?.Invoke();
        }
    }

    /// <summary>
    /// 冷却结束后才可使用
    /// </summary>
    public virtual bool CanUse()
    {
        return cooldownTimer <= 0;
    }

    /// <summary>
    /// 启动冷却计时并触发 OnSkillUsed 事件
    /// </summary>
    protected void StartCooldown()
    {
        cooldownTimer = Data.cooldown;
        OnSkillUsed?.Invoke();
    }

    public float CurrentCooldown => cooldownTimer;

    /// <summary>
    /// 冷却进度比例（0=就绪，1=刚释放），供 UI 填充用
    /// </summary>
    public float CooldownPercent
    {
        get
        {
            if (Data.cooldown <= 0)
            {
                return 0;
            }
            return cooldownTimer / Data.cooldown;
        }
    }

    /// <summary>
    /// 释放技能，子类实现具体逻辑
    /// </summary>
    public abstract void Use();

    public SkillData SkillData
    {
        get { return Data; }
    }
}