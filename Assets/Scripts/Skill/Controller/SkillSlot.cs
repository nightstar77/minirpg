using UnityEngine;

[System.Serializable]
public class SkillSlot
{
    [Header("技能")]
    [SerializeField]
    private SkillBase skill;

    [Header("快捷键")]
    [SerializeField]
    private KeyCode key;
    public SkillBase Skill => skill;
    public KeyCode Key => key;

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

    public void Clear()
    {
        skill = null;
    }

    public bool IsEmpty()
    {
        return skill == null;
    }

}