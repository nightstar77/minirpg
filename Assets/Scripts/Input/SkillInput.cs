using UnityEngine;

/// <summary>
/// 技能输入处理：监听 Q/E/R/F 键，将按键映射到对应技能槽并触发技能
/// </summary>
public class SkillInput : MonoBehaviour
{
    [SerializeField]
    private SkillController skillController;

    // 默认技能快捷键，顺序对应技能槽 0~3
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