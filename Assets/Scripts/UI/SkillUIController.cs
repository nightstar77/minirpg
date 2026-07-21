using UnityEngine;
using UnityEngine.UI;

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


        skill.OnSkillUsed -= StartCooldownUI;

        skill.OnCooldownFinished -= FinishCooldownUI;
    }
}