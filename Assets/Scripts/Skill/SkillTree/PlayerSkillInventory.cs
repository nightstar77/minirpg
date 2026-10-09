using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 玩家技能背包：管理已解锁技能列表，使用 PlayerPrefs 持久化
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
    /// 序列化为 JSON 存入 PlayerPrefs
    /// </summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(new SkillSaveWrapper(unlockedSkills));

        PlayerPrefs.SetString("PlayerSkills", json);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// 从 PlayerPrefs 读取已解锁技能
    /// </summary>
    public void Load()
    {
        if (!PlayerPrefs.HasKey("PlayerSkills"))
        {
            return;
        }

        string json = PlayerPrefs.GetString("PlayerSkills");

        SkillSaveWrapper data = JsonUtility.FromJson<SkillSaveWrapper>(json);

        unlockedSkills = data.skills;
    }

    // JSON 序列化包装类
    [System.Serializable]
    private class SkillSaveWrapper
    {
        public List<SkillType> skills;
        public SkillSaveWrapper(List<SkillType> skills)
        {
            this.skills = skills;
        }
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteKey("PlayerSkills");
        PlayerPrefs.Save();
        Debug.Log("技能存档已清除");
    }

}