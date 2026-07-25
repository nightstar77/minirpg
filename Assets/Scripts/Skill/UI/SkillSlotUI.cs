using UnityEngine;
using UnityEngine.UI;
using System;

public class SkillSlotUI : MonoBehaviour
{
    [Header("对应技能槽")]
    [SerializeField]
    private int slotIndex;

    [Header("UI")]
    [SerializeField]
    private Image icon;

    [SerializeField]
    private Image cooldownMask;

    [SerializeField]
    private SkillController skillController;

    private void Start()
    {
        skillController.OnSkillChanged += Refresh;
        Refresh();
    }

    private void Update()
    {
        RefreshCooldown();
    }

    public void Refresh()
    {
        Debug.Log("UI使用SkillController:" + skillController.gameObject.name);
        SkillBase skill = skillController.GetSlotSkill(slotIndex);

        Debug.Log("刷新技能槽:" + slotIndex);
        if (skill == null)
        {
            Debug.Log("UI读取不到技能:" + slotIndex);
            icon.enabled = false;
            return;
        }
        Debug.Log("UI读取到技能:" + skill.SkillData.skillName);
        icon.enabled = true;
        icon.sprite = skill.SkillData.icon;

    }

    private void RefreshCooldown()
    {
        SkillBase skill = skillController.GetSlotSkill(slotIndex);

        if (skill == null)
        {
            cooldownMask.enabled = false;
            return;
        }

        float percent = skill.CooldownPercent;
        cooldownMask.enabled = percent > 0;
        cooldownMask.fillAmount = percent;
    }

    private void OnDestroy()
    {
        if (skillController != null)
        {
            skillController.OnSkillChanged -= Refresh;
        }
    }

}