using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 技能冷却 UI 控制器：订阅技能使用/冷却结束事件，通过 Image fillAmount 显示冷却进度
/// </summary>
public class SkillUIController : MonoBehaviour
{
    [Header("绑定技能")]
    public SkillBase skill;


    [Header("UI")]
    public Image cooldownImage;


    private void Start()
    {
        if (skill == null)
        {
            Debug.LogError("没有绑定Skill");
            return;
        }
        skill.OnSkillUsed += StartCooldownUI;
        skill.OnCooldownFinished += FinishCooldownUI;
    }


    private void Update()
    {
        if (skill == null)
        {
            return;
        }
        UpdateCooldownUI();
    }


    private void UpdateCooldownUI()
    {
        cooldownImage.fillAmount = skill.CooldownPercent;
    }

    private void StartCooldownUI()
    {
        cooldownImage.gameObject.SetActive(true);
    }
    private void FinishCooldownUI()
    {
        cooldownImage.fillAmount = 0;

        cooldownImage.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (skill == null)
        {
            return;
        }

        // 取消订阅避免空引用
        skill.OnSkillUsed -= StartCooldownUI;

        skill.OnCooldownFinished -= FinishCooldownUI;
    }
}