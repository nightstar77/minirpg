using UnityEngine;
using System;

public abstract class SkillBase : MonoBehaviour
{
    protected abstract SkillData Data { get; }

    public event Action OnSkillUsed;
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
    public virtual bool CanUse()
    {
        return cooldownTimer <= 0;
    }
    protected void StartCooldown()
    {
        cooldownTimer = Data.cooldown;
        OnSkillUsed?.Invoke();
    }

    public float CurrentCooldown => cooldownTimer;

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

    public abstract void Use();
}