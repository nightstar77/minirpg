using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 技能槽 UI：显示技能图标与快捷键，实时更新冷却遮罩。
/// 点击槽位 → 弹出技能选择面板，选择已解锁技能装备到本槽。
/// 悬停时高亮显示。
/// </summary>
public class SkillSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("对应技能槽")]
    [SerializeField]
    private int slotIndex;

    [Header("UI")]
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Image cooldownMask;        // 冷却遮罩，fillAmount 控制遮盖比例
    [SerializeField]
    private TMP_Text keyText;
    [SerializeField]
    private TMP_Text cooldownText;     // 冷却剩余秒数文字

    [Header("悬停效果")]
    [Tooltip("悬停时图标颜色（轻微变亮）")]
    public Color hoverColor = new Color(1f, 1f, 0.8f);
    // 记录原始颜色，悬停结束后恢复
    private Color originalIconColor;
    // 是否正处于悬停状态
    private bool isHovering;

    [SerializeField]
    private SkillController skillController;

    private void Awake()
    {
        // 记录原始图标颜色，用于悬停恢复
        if (icon != null)
        {
            originalIconColor = icon.color;
        }
    }

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

        // 冷却遮罩按比例填充，同时显示剩余秒数
        float percent = skill.CooldownPercent;
        if (percent > 0.05f)
        {
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
        // 取消订阅避免空引用
        if (skillController != null)
        {
            skillController.OnSkillChanged -= Refresh;
        }
    }

    #region ===== 点击装备 =====

    /// <summary>
    /// 点击槽位 → 打开技能选择面板，从已解锁技能中选择一个装备到本槽
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        SkillEquipPanelUI.Instance.Open(slotIndex, skillController);
    }

    #endregion

    #region ===== 悬停高亮 =====

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        if (icon != null && icon.enabled)
        {
            icon.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (icon != null)
        {
            icon.color = originalIconColor;
        }
    }

    #endregion
}
