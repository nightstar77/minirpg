using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    [SerializeField]
    private SkillController skillController;

    private void Awake()
    {
        skillController = GetComponent<SkillController>();
    }

    public bool UnlockSkill(SkillNodeData node, int slotIndex)
    {
        // 空节点检查
        if (node == null)
        {
            return false;
        }

        // 已解锁
        if (node.unlocked)
        {
            Debug.Log("技能已经解锁");
            return false;
        }

        // 前置技能检查
        if (node.prerequisite != null)
        {
            if (!node.prerequisite.unlocked)
            {
                Debug.Log("前置技能未解锁");
                return false;
            }
        }

        // 解锁
        node.unlocked = true;

        // 装备技能
        skillController.EquipSkill(slotIndex, node.skill.skillType);

        Debug.Log("解锁技能:" + node.skill.skillName);
        return true;
    }

}