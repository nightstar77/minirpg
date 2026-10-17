using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 技能树节点 UI（重构版）：单个物体承担全部交互，无 Button、无子物体拖拽脚本。
///
/// 设计要点：
/// 1. 解锁用 OnPointerDown（按下即触发），不用 OnPointerClick。
///    因为只要物体上存在拖拽接口，按下时鼠标轻微移动就会被 Unity 判定为拖拽并取消点击，
///    而 PointerDown 在拖拽判定之前触发，100% 可靠。
/// 2. 拖拽用 IBeginDrag/IDrag/IEndDrag，仅已解锁技能生效，用于装备到技能槽。
/// 3. 悬停用 IPointerEnter/Exit 显示技能描述。
/// 4. 实现 ISkillDragSource，技能槽通过接口读取拖拽数据，不依赖具体拖拽源类型。
/// </summary>
[RequireComponent(typeof(Image))]
public class SkillNodeUI : MonoBehaviour,
    IPointerDownHandler,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    ISkillDragSource
{
    [Header("技能数据")]
    public SkillNodeData nodeData;

    [Header("UI")]
    [Tooltip("节点图标 Image，默认取自身上的 Image 组件")]
    public Image icon;

    [SerializeField]
    private SkillTooltipUI tooltip;

    [Header("提示框位置")]
    [Tooltip("提示框相对于技能图标的偏移（向下为负 Y）。由 SkillTreeUIManager 统一设置。")]
    public Vector2 tooltipOffset = new Vector2(0, -80f);

    [Header("拖拽设置")]
    [Tooltip("拖拽时原图标透明度")]
    [Range(0f, 1f)]
    public float dragAlpha = 0.4f;

    private SkillTreeManager skillTreeManager;
    private SkillNodeState currentState;
    private SkillDragPayload payload;
    private Canvas canvas;

    // 跟随鼠标的拖拽图标（临时物体，拖拽结束即销毁）
    private GameObject dragIcon;
    private RectTransform dragIconRect;

    /// <summary>拖拽数据，供技能槽 OnDrop 读取</summary>
    public SkillDragPayload Payload => payload;

    public SkillNodeState CurrentState => currentState;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        if (icon == null)
        {
            icon = GetComponent<Image>();
        }
    }

    public void Init(SkillNodeData data, SkillTreeManager manager)
    {
        nodeData = data;
        skillTreeManager = manager;

        payload = new SkillDragPayload
        {
            skillType = data.skill.skillType,
            iconSprite = data.skill.icon,
            skillName = data.skill.skillName,
            originalColor = Color.white
        };

        Refresh();
    }

    /// <summary>
    /// 根据节点状态刷新图标颜色：已解锁原色，未解锁灰色
    /// </summary>
    public void Refresh()
    {
        currentState = skillTreeManager.GetSkillState(nodeData);
        icon.sprite = nodeData.skill.icon;
        icon.color = currentState == SkillNodeState.Unlocked ? Color.white : Color.gray;
    }

    #region ===== 按下解锁 =====

    /// <summary>
    /// 按下即解锁（不用 OnPointerClick，避免拖拽判定吞掉点击）。
    /// 仅 Locked 状态（前置满足、技能点足够）可解锁。
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentState != SkillNodeState.Locked) { return; }

        if (skillTreeManager.UnlockSkill(nodeData))
        {
            Refresh();
        }
    }

    #endregion

    #region ===== 悬停提示 =====

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip == null) { return; }

        tooltip.Show(nodeData, skillTreeManager.GetSkillState(nodeData));
        // 提示框显示在图标下方偏移位置，避免遮挡图标
        tooltip.Move((Vector2)transform.position + tooltipOffset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip == null) { return; }
        tooltip.Hide();
    }

    #endregion

    #region ===== 拖拽装备 =====

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 仅已解锁技能可拖拽装备
        if (currentState != SkillNodeState.Unlocked) { return; }

        // 刷新拖拽数据（图标可能被改色，以当前为准）
        payload.iconSprite = icon.sprite;
        payload.originalPosition = transform.position;
        payload.originalSize = ((RectTransform)transform).sizeDelta;
        payload.originalColor = icon.color;

        // 原图标变半透明表示"被拿起"
        Color c = icon.color;
        c.a = dragAlpha;
        icon.color = c;

        CreateDragIcon();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIconRect != null)
        {
            dragIconRect.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 恢复原图标颜色
        icon.color = payload.originalColor;

        // 销毁拖拽图标（落在技能槽上时，装备逻辑由技能槽的 OnDrop 处理）
        if (dragIcon != null)
        {
            Destroy(dragIcon);
            dragIcon = null;
            dragIconRect = null;
        }
    }

    /// <summary>
    /// 创建跟随鼠标的拖拽图标：纯 Image，不接收射线，避免挡住技能槽的 OnDrop
    /// </summary>
    private void CreateDragIcon()
    {
        dragIcon = new GameObject("SkillDragIcon");
        dragIcon.transform.SetParent(canvas.transform, false);

        Image img = dragIcon.AddComponent<Image>();
        img.sprite = payload.iconSprite;
        img.raycastTarget = false;
        img.preserveAspect = true;

        dragIconRect = dragIcon.GetComponent<RectTransform>();
        dragIconRect.sizeDelta = payload.originalSize;
        dragIconRect.position = (Vector2)transform.position;
    }

    #endregion
}
