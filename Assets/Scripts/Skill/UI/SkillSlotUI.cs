using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlotUI : MonoBehaviour, IDropHandler
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
    private TMP_Text keyText;
    [SerializeField]
    private TMP_Text cooldownText;

    [SerializeField]
    private SkillController skillController;
    private SkillBase currentSkill;

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
        SkillBase skill = skillController.GetSlotSkill(slotIndex);

        Debug.Log("刷新技能槽:" + slotIndex);
        if (skill == null)
        {
            Debug.Log("UI读取不到技能:" + slotIndex);
            cooldownText.text = "";
            icon.enabled = false;
            return;
        }
        Debug.Log("UI读取到技能:" + skill.SkillData.skillName);
        icon.enabled = true;
        icon.sprite = skill.SkillData.icon;
        keyText.text = skillController.GetSlotKey(slotIndex).ToString();
    }

    private void RefreshCooldown()
    {
        SkillBase skill = skillController.GetSlotSkill(slotIndex);

        if (skill == null)
        {
            cooldownMask.enabled = false;
            cooldownText.text = "";
            return;
        }

        //冷却遮盖效果
        float percent = skill.CooldownPercent;
        if (percent > 0.05f)
        {
            Debug.Log("冷却:" + skill.CurrentCooldown);
            cooldownMask.enabled = true;
            cooldownMask.fillAmount = percent;
            cooldownText.enabled = true;
            int time = Mathf.CeilToInt(skill.CurrentCooldown);
            cooldownText.text = time.ToString();
        }
        else
        {
            cooldownMask.enabled = false;
            cooldownText.enabled = false;
            cooldownText.text = "";
        }
    }

    private void OnDestroy()
    {
        if (skillController != null)
        {
            skillController.OnSkillChanged -= Refresh;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        SkillDragHandler drag = eventData.pointerDrag.GetComponent<SkillDragHandler>();

        if (drag == null) { return; }
        SkillBase skill = skillController.GetSkill(drag.SkillType);

        if (skill == null)
        {
            Debug.Log("没有找到技能");
            return;
        }
        skillController.EquipSkill(slotIndex, skill);
    }

}