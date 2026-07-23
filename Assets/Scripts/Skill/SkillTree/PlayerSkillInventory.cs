using UnityEngine;
using System.Collections.Generic;

public class PlayerSkillInventory : MonoBehaviour
{
    private List<SkillType> unlockedSkills = new List<SkillType>();

    public bool HasSkill(SkillType type)
    {
        return unlockedSkills.Contains(type);
    }

    public void AddSkill(SkillType type)
    {
        if (HasSkill(type)) { return; }

        unlockedSkills.Add(type);

        Debug.Log("获得技能:" + type);
    }

    public IReadOnlyList<SkillType> GetSkills()
    {
        return unlockedSkills;
    }

}