using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// 技能树节点 UI：显示技能图标，根据状态着色，点击解锁，悬停显示信息提示
/// </summary>
public class SkillNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("技能数据")]
    public SkillNodeData nodeData;

    [Header("UI")]
    public Image icon;

    [SerializeField]
    private SkillTooltipUI tooltip;

    [Header("管理器")]
    private SkillTreeManager skillTreeManager;
    private Button button;
    private SkillNodeState currentState;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void Init(SkillNodeData data, SkillTreeManager manager)
    {
        nodeData = data;
        skillTreeManager = manager;
        SkillDragHandler drag = GetComponentInChildren<SkillDragHandler>();
        if (drag != null)
        {
            // 初始化拖拽处理器：传入技能类型和图标引用
            drag.Init(data.skill.skillType, icon);
        }
        Refresh();
    }

    private void OnClick()
    {
        // 仅 Locked 状态（可学习）时才能点击解锁
        if (currentState != SkillNodeState.Locked) { return; }
        bool result = skillTreeManager.UnlockSkill(nodeData);

        if (result) { Refresh(); }
    }

    /// <summary>
    /// 根据节点状态刷新图标颜色、按钮可交互性和拖拽可用性
    /// 已解锁：图标原色；未解锁（包括缺前置、缺技能点、可学习）：灰色
    /// </summary>
    public void Refresh()
    {
        currentState = skillTreeManager.GetSkillState(nodeData);

        // 已解锁 → 原色；所有未解锁状态 → 灰色
        if (currentState == SkillNodeState.Unlocked)
        {
            icon.color = Color.white;
        }
        else
        {
            icon.color = Color.gray;
        }

        // 仅 Locked 状态可点击（点击解锁）
        button.interactable = currentState == SkillNodeState.Locked;

        icon.sprite = nodeData.skill.icon;

        // 更新拖拽可用性：仅已解锁的技能可以被拖拽装备
        SkillDragHandler drag = GetComponentInChildren<SkillDragHandler>();
        if (drag != null)
        {
            drag.SetCanDrag(currentState == SkillNodeState.Unlocked);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SkillNodeState state = skillTreeManager.GetSkillState(nodeData);
        tooltip.Show(nodeData, state);
        tooltip.Move(Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
    }

    public SkillNodeState CurrentState
    {
        get { return currentState; }
    }

}