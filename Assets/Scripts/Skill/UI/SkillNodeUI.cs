using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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
            drag.Init(data, icon);
        }
        Refresh();
    }

    private void OnClick()
    {
        if (currentState != SkillNodeState.Locked) { return; }
        bool result = skillTreeManager.UnlockSkill(nodeData);

        if (result) { Refresh(); }
    }

    public void Refresh()
    {
        currentState = skillTreeManager.GetSkillState(nodeData);

        switch (currentState)
        {
            case SkillNodeState.Unlocked:
                icon.color = Color.green;
                break;

            case SkillNodeState.NeedPrerequisite:
                icon.color = Color.red;
                break;

            case SkillNodeState.NeedPoint:
                icon.color = Color.yellow;
                break;

            case SkillNodeState.Locked:
                icon.color = Color.gray;
                break;
        }
        button.interactable = currentState == SkillNodeState.Locked;

        //图标
        icon.sprite = nodeData.skill.icon;
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