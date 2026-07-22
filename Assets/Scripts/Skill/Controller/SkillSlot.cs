using UnityEngine;

[System.Serializable]
public class SkillSlot
{
    [SerializeField]
    private SkillBase skill;

    public SkillBase Skill => skill;

    public void SetSkill(SkillBase newSkill)
    {
        skill = newSkill;
        Debug.Log("SkillSlot绑定:" + skill.name);
    }

    public void Use()
    {
        if (skill == null)
        {
            Debug.Log("技能槽为空");
            return;
        }
        skill.Use();
    }

}