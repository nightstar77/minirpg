using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [Header("技能槽")]
    [SerializeField]
    private SkillSlot[] skillSlots;

    private Dictionary<SkillType, SkillBase> skillMap = new Dictionary<SkillType, SkillBase>();

    private void Awake()
    {
        RegisterSkills();
    }

    private void RegisterSkills()
    {
        SkillBase[] skills = GetComponents<SkillBase>();
        foreach (SkillBase skill in skills)
        {
            if (skill == null)
            {
                continue;
            }

            if (!skillMap.ContainsKey(skill.SkillType))
            {
                skillMap.Add(skill.SkillType, skill);
            }

            Debug.Log("注册技能：" + skill.SkillType);
        }
    }

    /// <summary>
    /// 根据技能类型获取技能
    /// </summary>
    public SkillBase GetSkill(SkillType type)
    {
        if (skillMap.TryGetValue(type, out SkillBase skill))
        {
            return skill;
        }
        Debug.LogWarning("没有找到技能：" + type);

        return null;
    }

    public void EquipSkill(int slotIndex, SkillType type)
    {
        SkillBase skill = GetSkill(type);

        if (skill == null)
        {
            Debug.LogWarning("没有找到技能:" + type);

            return;
        }

        if (slotIndex < 0 || slotIndex >= skillSlots.Length)
        {
            return;
        }

        skillSlots[slotIndex].SetSkill(skill);
        Debug.Log("装备技能:" + type + " 到槽位:" + slotIndex);
    }

    public void UseSkill(int slotIndex)
    {
        Debug.Log("调用技能槽:" + slotIndex);
        if (slotIndex < 0 || slotIndex >= skillSlots.Length)
        {
            Debug.LogWarning("技能槽不存在");
            return;
        }
        skillSlots[slotIndex].Use();
    }

}