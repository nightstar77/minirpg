using UnityEngine;

/// <summary>
/// 技能装备管理器：检查玩家是否拥有技能后委托 SkillController 执行装备
/// </summary>
public class SkillEquipManager : MonoBehaviour
{
    [SerializeField]
    private PlayerSkillInventory inventory;

    [SerializeField]
    private SkillController skillController;

    private void Awake()
    {
        if (inventory == null)
        {
            inventory = GetComponent<PlayerSkillInventory>();
        }

        if (skillController == null)
        {
            skillController = GetComponent<SkillController>();
        }
    }

    /// <summary>
    /// 装备技能到指定槽位（需先检查是否拥有该技能）
    /// </summary>
    public bool EquipSkill(SkillType type, int slotIndex)
    {
        // 检查是否拥有技能
        if (!inventory.HasSkill(type))
        {
            Debug.Log("玩家没有该技能:" + type);
            return false;
        }

        // 装备
        skillController.EquipSkill(slotIndex, type);
        return true;
    }

}