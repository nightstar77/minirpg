using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 技能槽 UI：显示技能图标与快捷键，实时更新冷却遮罩。
/// 支持拖拽交互：
/// - 从技能树/技能槽拖入 → 装备或交换
/// - 从本槽拖出 → 拖到另一个槽位进行交换
/// 悬停时高亮显示。
/// </summary>
public class SkillSlotUI : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
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
    // 本槽位的拖拽处理器（挂在 icon 子对象上，运行时自动添加）
    private SkillDragHandler dragHandler;

    private void Awake()
    {
        // 确保 icon 上有 SkillDragHandler 组件，没有则自动添加（支持从槽位拖出）
        if (icon != null)
        {
            dragHandler = icon.GetComponent<SkillDragHandler>();
            if (dragHandler == null)
            {
                dragHandler = icon.gameObject.AddComponent<SkillDragHandler>();
            }
        }
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
            // 空槽位不可拖拽
            if (dragHandler != null)
            {
                dragHandler.SetCanDrag(false);
            }
            return;
        }
        Debug.Log("UI读取到技能:" + skill.SkillData.skillName);
        icon.enabled = true;
        icon.sprite = skill.SkillData.icon;
        keyText.text = skillController.GetSlotKey(slotIndex).ToString();

        // 更新拖拽数据：技能类型 + 图标引用，并启用拖拽
        if (dragHandler != null)
        {
            dragHandler.Init(skill.SkillType, icon);
            dragHandler.SetCanDrag(true);
        }
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

    #region ===== 拖拽接收（OnDrop）=====

    /// <summary>
    /// 接收拖拽：通过 SkillDragPayload 获取技能数据，判断来源类型后执行装备或交换
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        // 从拖拽源获取 SkillDragHandler
        SkillDragHandler drag = eventData.pointerDrag.GetComponent<SkillDragHandler>();
        if (drag == null) { return; }

        // 从 Payload 读取技能数据，而不是直接访问私有字段
        SkillDragPayload data = drag.Payload;
        if (data.skillType == SkillType.None)
        {
            Debug.LogWarning("拖拽数据无效");
            return;
        }

        // 检查拖拽源是否来自另一个技能槽（向上找 SkillSlotUI 组件）
        SkillSlotUI sourceSlot = eventData.pointerDrag.GetComponentInParent<SkillSlotUI>();

        if (sourceSlot != null && sourceSlot != this)
        {
            // 来源是另一个技能槽 → 交换两个槽位的技能
            Debug.Log($"从槽位 {sourceSlot.slotIndex} 拖拽到槽位 {slotIndex}，执行交换");
            skillController.SwapSkills(sourceSlot.slotIndex, slotIndex);
        }
        else if (sourceSlot == null)
        {
            // 来源是技能树节点 → 直接装备技能到本槽位
            SkillBase skill = skillController.GetSkill(data.skillType);
            if (skill == null)
            {
                Debug.Log("没有找到技能:" + data.skillType);
                return;
            }
            skillController.EquipSkill(slotIndex, skill);
        }
        // 如果 sourceSlot == this（拖到自己身上），什么都不做
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
