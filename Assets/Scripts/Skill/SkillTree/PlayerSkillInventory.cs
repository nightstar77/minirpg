using UnityEngine;
using System.Collections.Generic;

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

    public void Save()
    {
        string json = JsonUtility.ToJson(new SkillSaveWrapper(unlockedSkills));

        PlayerPrefs.SetString("PlayerSkills", json);

        PlayerPrefs.Save();
    }

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