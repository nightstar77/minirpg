using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能树管理器：检查节点状态、消耗技能点解锁技能、验证前置条件
/// </summary>
public class SkillTreeManager : MonoBehaviour
{
    [Header("技能系统")]
    [SerializeField]
    private SkillController skillController;
    // 技能解锁事件，UI 订阅后刷新节点和连线
    public event Action OnSkillUnlocked;

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

    /// <summary>
    /// 获取技能节点当前状态（已解锁/需前置/需技能点/可学习）
    /// </summary>
    public SkillNodeState GetSkillState(SkillNodeData node)
    {
        if (playerSkillInventory.HasSkill(node.skill.skillType))
        { return SkillNodeState.Unlocked; }

        if (!CheckPrerequisite(node))
        { return SkillNodeState.NeedPrerequisite; }

        if (!skillPoint.CanSpend(node.cost))
        { return SkillNodeState.NeedPoint; }

        return SkillNodeState.Locked;
    }

    /// <summary>
    /// 解锁技能：依次检查空节点、已解锁、前置、技能点，通过后消耗技能点并添加到背包
    /// </summary>
    public bool UnlockSkill(SkillNodeData node)
    {
        // 空节点检查
        if (node == null)
        {
            Debug.Log("技能节点为空");
            return false;
        }

        // 已解锁
        if (playerSkillInventory.HasSkill(node.skill.skillType))
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
        if (!skillPoint.CanSpend(node.cost))
        {
            Debug.Log("技能点不足");
            return false;
        }

        // 解锁：消耗技能点 + 添加到背包
        skillPoint.SpendPoint(node.cost);
        playerSkillInventory.AddSkill(node.skill.skillType);

        OnSkillUnlocked?.Invoke();
        Debug.Log("解锁技能:" + node.skill.skillName);
        return true;
    }

    /// <summary>
    /// 检查前置技能是否全部已解锁
    /// </summary>
    private bool CheckPrerequisite(SkillNodeData node)
    {
        if (node.prerequisites == null)
        { return true; }

        foreach (SkillNodeData pre in node.prerequisites)
        {
            if (!playerSkillInventory.HasSkill(pre.skill.skillType))
            {
                Debug.Log("前置技能未解锁:" + pre.skill.skillName);
                return false;
            }
        }
        return true;
    }

    public bool HasSkill(SkillType type)
    {
        return playerSkillInventory.HasSkill(type);
    }

}