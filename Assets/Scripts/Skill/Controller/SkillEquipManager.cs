using UnityEngine;

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

    public bool EquipSkill(SkillType type, int slotIndex)
    {
        //检查是否拥有技能
        if (!inventory.HasSkill(type))
        {
            Debug.Log("玩家没有该技能:" + type);
            return false;
        }

        //装备
        skillController.EquipSkill(slotIndex, type);

        Debug.Log("装备技能:" + type + " 到槽位:" + slotIndex);

        return true;
    }

}