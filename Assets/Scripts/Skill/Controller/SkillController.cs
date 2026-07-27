using System.Collections.Generic;
using UnityEngine;
using System;

public class SkillController : MonoBehaviour
{
    public event Action OnSkillChanged;

    [Header("技能槽")]
    [SerializeField]
    private SkillSlot[] skillSlots;
    private Dictionary<SkillType, SkillBase> skillMap = new Dictionary<SkillType, SkillBase>();

    [SerializeField]
    private PlayerSkillInventory inventory;

    private void Awake()
    {
        inventory = GetComponent<PlayerSkillInventory>();
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
        { return skill; }
        Debug.LogWarning("没有找到技能：" + type);
        return null;
    }

    public bool EquipSkill(int slotIndex, SkillType type)
    {
        //检查玩家是否拥有技能
        if (!inventory.HasSkill(type))
        {
            Debug.Log("玩家没有该技能:" + type);
            return false;
        }

        SkillBase skill = GetSkill(type);

        if (skill == null)
        {
            Debug.LogWarning("没有找到技能:" + type);
            return false;
        }

        if (slotIndex < 0 || slotIndex >= skillSlots.Length)
        {
            Debug.LogWarning("技能槽索引错误");
            return false;
        }

        if (skillSlots[slotIndex] == null)
        {
            Debug.LogWarning("技能槽为空:" + slotIndex);
            return false;
        }
        skillSlots[slotIndex].SetSkill(skill);

        OnSkillChanged?.Invoke();

        Debug.Log("装备技能:" + type + " 到槽位:" + slotIndex + " 当前对象:" + gameObject.name);
        return true;
    }

    public void EquipSkill(int slotIndex, SkillBase skill)
    {
        if (slotIndex < 0 || slotIndex >= skillSlots.Length) { return; }
        skillSlots[slotIndex].SetSkill(skill);

        OnSkillChanged?.Invoke();
        Debug.Log("替换技能槽:" + slotIndex + " 技能:" + skill.SkillData.skillName);
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

    public SkillBase GetSlotSkill(int index)
    {
        if (index < 0 || index >= skillSlots.Length)
        {
            Debug.LogWarning("技能槽索引错误");
            return null;
        }
        return skillSlots[index].Skill;
    }

    public int GetSlotIndexByKey(KeyCode key)
    {
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots[i].Key == key)
            { return i; }
        }
        return -1;
    }

    public KeyCode GetSlotKey(int index)
    {
        if (index < 0 || index >= skillSlots.Length)
        {
            Debug.LogWarning("技能槽索引错误");
            return KeyCode.None;
        }
        return skillSlots[index].Key;
    }

}