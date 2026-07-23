using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    [Header("技能系统")]
    [SerializeField]
    private SkillController skillController;

    [Header("技能点")]
    [SerializeField]
    private PlayerSkillPoint skillPoint;

    [SerializeField]
    private PlayerSkillInventory playerSkillInventory;

    private void Awake()
    {
        skillController = GetComponent<SkillController>();
        playerSkillInventory = GetComponent<PlayerSkillInventory>();
    }

    public SkillNodeState GetSkillState(SkillNodeData node)
    {
        if (node.unlocked)
        { return SkillNodeState.Unlocked; }

        if (!CheckPrerequisite(node))
        { return SkillNodeState.NeedPrerequisite; }

        if (!skillPoint.CanSpend(node.cost))
        { return SkillNodeState.NeedPoint; }

        return SkillNodeState.Locked;
    }

    public bool UnlockSkill(SkillNodeData node)
    {
        // 空节点检查
        if (node == null)
        {
            Debug.Log("技能节点为空");
            return false;
        }

        // 已解锁
        if (node.unlocked)
        {
            Debug.Log("技能已经解锁");
            return false;
        }

        // 前置技能检查
        if (!CheckPrerequisite(node))
        {
            Debug.Log("前置技能未解锁");
            return false;
        }

        // 技能点检查
        if (!skillPoint.SpendPoint(node.cost))
        {
            Debug.Log("技能点不足");
            return false;
        }

        // 解锁
        node.unlocked = true;
        playerSkillInventory.AddSkill(node.skill.skillType);

        Debug.Log("解锁技能:" + node.skill.skillName);
        return true;
    }

    private bool CheckPrerequisite(SkillNodeData node)
    {
        if (node.prerequisites == null)
        { return true; }

        foreach (SkillNodeData pre in node.prerequisites)
        {
            if (!pre.unlocked)
            {
                Debug.Log("前置技能未解锁:" + pre.skill.skillName);
                return false;
            }
        }
        return true;
    }

}