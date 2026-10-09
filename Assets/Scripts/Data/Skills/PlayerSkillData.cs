using System;
using System.Collections.Generic;

/// <summary>
/// 玩家技能存档数据，序列化为 JSON 存入 PlayerPrefs
/// </summary>
[Serializable]
public class PlayerSkillData
{
    public List<SkillType> unlockedSkills = new List<SkillType>();
}