using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 技能树节点 UI：单个物体承担全部交互，无 Button、无子物体拖拽脚本。
///
/// 设计要点：
/// 1. 解锁用 OnPointerDown（按下即触发），不用 OnPointerClick。
///    因为只要物体上存在拖拽接口，按下时鼠标轻微移动就会被 Unity 判定为拖拽并取消点击，
///    而 PointerDown 在拖拽判定之前触发，100% 可靠。
/// 2. 悬停用 IPointerEnter/Exit 显示技能描述。
/// 3. 状态实时查询，不用缓存：升级获得技能点后节点不会自动 Refresh，缓存会过期。
/// </summary>
[RequireComponent(typeof(Image))]
public class SkillNodeUI : MonoBehaviour,
    IPointerDownHandler,
    IPointerEnterHandler, IPointerExitHandler
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

    private SkillTreeManager skillTreeManager;
    private SkillNodeState currentState;

    public SkillNodeState CurrentState => currentState;

    private void Awake()
    {
        if (icon == null)
        {
            icon = GetComponent<Image>();
        }
    }

    public void Init(SkillNodeData data, SkillTreeManager manager)
    {
        nodeData = data;
        skillTreeManager = manager;
        Refresh();
        Debug.Log($"[SkillNodeUI] 初始化节点 {data.skill.skillName}，初始状态: {currentState}");
    }

    /// <summary>
    /// 根据节点状态刷新图标颜色：已解锁原色，未解锁灰色
    /// </summary>
    public void Refresh()
    {
        currentState = skillTreeManager.GetSkillState(nodeData);
        icon.sprite = nodeData.skill.icon;
        icon.color = currentState == SkillNodeState.Unlocked ? Color.white : Color.gray;
        Debug.Log($"[SkillNodeUI] 刷新 {nodeData.skill.skillName} → 状态: {currentState}");
    }

    #region ===== 按下解锁 =====

    /// <summary>
    /// 按下即解锁（不用 OnPointerClick，避免拖拽判定吞掉点击）。
    /// 仅 Locked 状态（前置满足、技能点足够）可解锁。
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // 必须实时查询状态，不能用缓存的 currentState：
        // 升级获得技能点后节点不会自动 Refresh，缓存可能停留在 NeedPoint，
        // 导致明明可学习却永远跳过解锁
        currentState = skillTreeManager.GetSkillState(nodeData);
        Debug.Log($"[SkillNodeUI] 按下 {nodeData.skill.skillName}，当前状态: {currentState}");

        if (currentState != SkillNodeState.Locked)
        {
            Debug.Log($"[SkillNodeUI] {nodeData.skill.skillName} 状态是 {currentState}（不是 Locked 可学习状态），跳过解锁");
            return;
        }

        bool result = skillTreeManager.UnlockSkill(nodeData);
        Debug.Log($"[SkillNodeUI] 解锁{(result ? "成功" : "失败")}: {nodeData.skill.skillName}");

        if (result)
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
}
