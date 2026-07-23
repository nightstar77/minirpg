using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillNodeUI : MonoBehaviour
{
    [Header("技能数据")]
    public SkillNodeData nodeData;

    [Header("UI")]
    public Image icon;
    public TMP_Text skillName;
    public TMP_Text lockText;

    [Header("管理器")]
    public SkillTreeManager skillTreeManager;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void Init(SkillNodeData data, SkillTreeManager manager)
    {
        nodeData = data;
        skillTreeManager = manager;
        Refresh();
    }

    private void OnClick()
    {
        bool result = skillTreeManager.UnlockSkill(nodeData);

        if (result) { Refresh(); }
    }

    public void Refresh()
    {
        SkillNodeState state = skillTreeManager.GetSkillState(nodeData);

        switch (state)
        {

            case SkillNodeState.Unlocked:
                lockText.text = "已解锁";
                break;

            case SkillNodeState.NeedPrerequisite:
                lockText.text = "需要前置技能";
                break;

            case SkillNodeState.NeedPoint:
                lockText.text = "技能点不足";
                break;

            case SkillNodeState.Locked:
                lockText.text = "未解锁";
                break;
        }

        //图标
        icon.sprite = nodeData.skill.icon;

        //名字
        skillName.text = nodeData.skill.skillName;

        //状态
        if (nodeData.unlocked)
        { lockText.text = "Unlock"; }
        else
        { lockText.text = "Lock"; }
    }

    public void SetData(SkillNodeData data)
    {
        nodeData = data;
        Refresh();
    }

}