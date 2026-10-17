using UnityEngine;
using TMPro;

/// <summary>
/// 技能信息提示框：鼠标悬停时显示技能名称、描述、消耗和状态
/// </summary>
public class SkillTooltipUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private TMP_Text skillName;

    [SerializeField]
    private TMP_Text description;

    [SerializeField]
    private TMP_Text cost;

    [SerializeField]
    private TMP_Text state;

    private RectTransform rect;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        // 确保提示框不拦截任何点击事件（不遮挡技能图标的点击解锁）
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        Hide();
    }

    public void Show(SkillNodeData node, SkillNodeState skillState)
    {
        gameObject.SetActive(true);

        skillName.text = node.skill.skillName;
        description.text = node.skill.description;
        cost.text = "消耗技能点:" + node.cost;

        switch (skillState)
        {
            case SkillNodeState.Unlocked:
                state.text = "已解锁";
                break;

            case SkillNodeState.Locked:
                state.text = "可学习";
                break;

            case SkillNodeState.NeedPoint:
                state.text = "技能点不足";
                break;

            case SkillNodeState.NeedPrerequisite:
                state.text = "需要前置技能";
                break;
        }

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Move(Vector2 position)
    {
        rect.position = position;
    }

}