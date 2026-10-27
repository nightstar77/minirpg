using UnityEngine;
using System.Collections.Generic;
using MiniRPG.Core;

/// <summary>
/// 玩家技能背包：管理已解锁技能列表，通过 GameManager.SaveData 持久化。
/// </summary>
public class PlayerSkillInventory : MonoBehaviour
{
    private List<SkillType> unlockedSkills = new List<SkillType>();

    private void Awake()
    {
        Load();
    }

    public bool HasSkill(SkillType type)
    {
        return unlockedSkills.Contains(type);
    }

    public void AddSkill(SkillType type)
    {
        if (HasSkill(type)) { return; }

        unlockedSkills.Add(type);
        Save();

        Debug.Log("获得技能:" + type);
    }

    public IReadOnlyList<SkillType> GetSkills()
    {
        return unlockedSkills;
    }

    /// <summary>
    /// 从 GameManager.SaveData 读取已解锁技能
    /// </summary>
    public void Load()
    {
        unlockedSkills = new List<SkillType>();
        if (GameManager.Instance?.SaveData?.unlockedSkills == null) return;

        foreach (int raw in GameManager.Instance.SaveData.unlockedSkills)
        {
            SkillType type = (SkillType)raw;
            // 过滤掉已删除/无效的技能类型（如旧存档残留的 Defense）
            if (!System.Enum.IsDefined(typeof(SkillType), type))
            {
                Debug.Log($"[PlayerSkillInventory] 忽略无效技能存档: {raw}");
                continue;
            }
            if (!unlockedSkills.Contains(type))
            {
                unlockedSkills.Add(type);
            }
        }
    }

    /// <summary>
    /// 序列化到 GameManager.SaveData 并写盘
    /// </summary>
    public void Save()
    {
        var data = GameManager.Instance?.SaveData;
        if (data == null) return;

        data.unlockedSkills = new List<int>();
        foreach (SkillType type in unlockedSkills)
        {
            data.unlockedSkills.Add((int)type);
        }
        GameManager.Instance.Save();
    }

    public void ClearSave()
    {
        unlockedSkills.Clear();
        if (GameManager.Instance?.SaveData != null)
        {
            GameManager.Instance.SaveData.unlockedSkills.Clear();
            GameManager.Instance.Save();
        }
        Debug.Log("技能存档已清除");
    }
}
