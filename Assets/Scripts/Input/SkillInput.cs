using UnityEngine;

public class SkillInput : MonoBehaviour
{
    [SerializeField]
    private SkillController skillController;

    private KeyCode[] skillKeys = { KeyCode.Q, KeyCode.E, KeyCode.R, KeyCode.F };

    private void Awake()
    {
        if (skillController == null)
        { skillController = GetComponent<SkillController>(); }
    }

    private void Update()
    {
        foreach (KeyCode key in skillKeys)
        {
            if (Input.GetKeyDown(key))
            { UseSkillKey(key); }
        }
    }

    private void UseSkillKey(KeyCode key)
    {
        int index = skillController.GetSlotIndexByKey(key);
        if (index == -1)
        {
            Debug.Log("没有绑定技能槽:" + key);
            return;
        }
        skillController.UseSkill(index);
    }

}