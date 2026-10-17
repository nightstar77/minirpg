using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 技能槽 UI：显示技能图标与快捷键，冷却时显示剩余时间（0.1 秒精度）。
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
    private TMP_Text keyText;
    [SerializeField]
    private TMP_Text cooldownText;     // 冷却剩余时间文字（0.1s 精度）

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
        // 保证槽位永远有可点击区域：
        // 空槽时 icon.enabled=false，被禁用的 Image 不接收射线，会导致空槽点不上。
        // 所以根物体必须有一个常启用的 Image 专门负责接收点击（全透明也可以）。
        Image clickCatcher = GetComponent<Image>();
        if (clickCatcher == null)
        {
            clickCatcher = gameObject.AddComponent<Image>();
            // 全透明，看不见但接收射线
            clickCatcher.color = new Color(1f, 1f, 1f, 0f);
        }
        clickCatcher.raycastTarget = true;

        // 子元素只做显示，全部关闭射线接收，避免遮挡/干扰点击与悬停判定
        if (keyText != null) { keyText.raycastTarget = false; }
        if (cooldownText != null) { cooldownText.raycastTarget = false; }

        // 记录原始图标颜色，用于悬停恢复
        if (icon != null)
        {
            icon.raycastTarget = false;
            originalIconColor = icon.color;
        }
    }

    private void Start()
    {
        skillController.OnSkillChanged += Refresh;
        // 设置面板改键后刷新快捷键显示
        GameInputs.OnBindingsChanged += RefreshKeyText;
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
        keyText.text = GameInputs.GetKeyName(GameInputs.SkillActions[slotIndex]);
    }

    /// <summary>
    /// 改键后只刷新快捷键文本（不重复刷新图标）
    /// </summary>
    private void RefreshKeyText()
    {
        SkillBase skill = skillController.GetSlotSkill(slotIndex);
        if (skill != null)
        {
            keyText.text = GameInputs.GetKeyName(GameInputs.SkillActions[slotIndex]);
        }
    }

    private void RefreshCooldown()
    {
        SkillBase skill = skillController.GetSlotSkill(slotIndex);

        if (skill == null)
        {
            cooldownText.enabled = false;
            cooldownText.text = "";
            return;
        }

        // 冷却剩余时间，0.1 秒精度（如 3.2），低于 0.05s 视为冷却结束
        float remaining = skill.CurrentCooldown;
        if (remaining > 0.05f)
        {
            cooldownText.enabled = true;
            cooldownText.text = remaining.ToString("F1");
        }
        else
        {
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
        GameInputs.OnBindingsChanged -= RefreshKeyText;
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
