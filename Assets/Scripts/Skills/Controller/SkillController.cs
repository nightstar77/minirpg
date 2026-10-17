using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 技能控制器：管理技能注册、装备到技能槽、按键映射与技能释放。
/// 通过 SkillType → SkillBase 字典快速查找已注册技能。
/// </summary>
public class SkillController : MonoBehaviour
{
    // 技能槽变更事件，UI 订阅后刷新显示
    public event Action OnSkillChanged;

    [Header("技能槽")]
    [SerializeField]
    private SkillSlot[] skillSlots;
    // 已注册技能字典：SkillType → SkillBase 实例
    private Dictionary<SkillType, SkillBase> skillMap = new Dictionary<SkillType, SkillBase>();

    [SerializeField]
    private PlayerSkillInventory inventory;

    private void Awake()
    {
        inventory = GetComponent<PlayerSkillInventory>();
        RegisterSkills();
    }

    /// <summary>
    /// 自动注册同一 GameObject 上所有 SkillBase 子类
    /// </summary>
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
    /// 根据技能类型获取技能实例
    /// </summary>
    public SkillBase GetSkill(SkillType type)
    {
        if (skillMap.TryGetValue(type, out SkillBase skill))
        { return skill; }
        Debug.LogWarning("没有找到技能：" + type);
        return null;
    }

    /// <summary>
    /// 装备技能到指定槽位（需先检查玩家是否拥有该技能）
    /// </summary>
    public bool EquipSkill(int slotIndex, SkillType type)
    {
        // 检查玩家是否拥有技能
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

    /// <summary>
    /// 装备技能实例到指定槽位（同样校验玩家是否拥有该技能）
    /// 用于拖拽装备等直接持有 SkillBase 引用的场景
    /// </summary>
    public bool EquipSkill(int slotIndex, SkillBase skill)
    {
        if (slotIndex < 0 || slotIndex >= skillSlots.Length) { return false; }

        // 校验玩家是否拥有该技能，逻辑层统一把关，防止绕过UI直接调用
        if (skill == null || !inventory.HasSkill(skill.SkillType))
        {
            Debug.LogWarning("玩家未拥有该技能，无法装备:" + (skill != null ? skill.SkillType.ToString() : "null"));
            return false;
        }

        skillSlots[slotIndex].SetSkill(skill);

        OnSkillChanged?.Invoke();
        Debug.Log("替换技能槽:" + slotIndex + " 技能:" + skill.SkillData.skillName);
        return true;
    }

    /// <summary>
    /// 交换两个技能槽的技能（用于拖拽互换）
    /// </summary>
    public bool SwapSkills(int slotA, int slotB)
    {
        if (slotA < 0 || slotA >= skillSlots.Length ||
            slotB < 0 || slotB >= skillSlots.Length)
        {
            Debug.LogWarning("交换技能槽失败：索引越界");
            return false;
        }

        SkillBase skillA = skillSlots[slotA].Skill;
        SkillBase skillB = skillSlots[slotB].Skill;

        // 两个槽位都空则无需交换
        if (skillA == null && skillB == null)
        { return false; }

        // 交换（两边都是已装备的技能，拥有性已校验过，无需重复检查）
        skillSlots[slotA].SetSkill(skillB);
        skillSlots[slotB].SetSkill(skillA);

        OnSkillChanged?.Invoke();
        Debug.Log($"交换技能槽 {slotA} ↔ {slotB}");
        return true;
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

    public KeyCode GetSlotKey(int index)
    {
        if (index < 0 || index >= skillSlots.Length)
        {
            Debug.LogWarning("技能槽索引错误");
            return KeyCode.None;
        }
        // 按键以 GameInputs 为准（设置面板改键后实时生效）
        return GameInputs.GetSkillKey(index);
    }

}