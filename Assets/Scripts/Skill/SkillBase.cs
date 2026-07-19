using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    protected abstract SkillData Data { get; }
    protected bool isCoolingDown;

    public virtual bool CanUse()
    {
        return !isCoolingDown;
    }
    protected void StartCooldown()
    {
        isCoolingDown = true;

        Invoke(nameof(ResetCooldown), Data.cooldown);
    }
    private void ResetCooldown()
    {
        isCoolingDown = false;
    }
    public abstract void Use();
}