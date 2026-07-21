using UnityEngine;
using System.Collections.Generic;


public class SkillTreeManager : MonoBehaviour
{
    [Header("技能点")]
    public int skillPoints = 5;

    [Header("技能节点")]
    public List<SkillNodeData> skillNodes;

    public bool UnlockSkill(SkillNodeData node)
    {
        Debug.Log("尝试解锁技能");
        Debug.Log(node);
        //已经解锁
        if (node.unlocked)
        {
            Debug.Log("技能已经解锁");
            return false;
        }

        //检查技能点
        if (skillPoints < node.cost)
        {
            Debug.Log("技能点不足");
            return false;
        }

        //检查前置技能
        if (node.prerequisite != null &&
           !node.prerequisite.unlocked)
        {
            Debug.Log("前置技能未解锁");
            return false;
        }

        //扣除技能点
        skillPoints -= node.cost;

        //解锁
        node.unlocked = true;
        Debug.Log($"解锁技能:{node.skill.skillName}");
        return true;
    }
}