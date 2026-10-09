using UnityEngine;

/// <summary>
/// 测试脚本：按5键查询0号槽当前装备的技能
/// </summary>
public class SkillInfoTest : MonoBehaviour
{
    public SkillController controller;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SkillBase skill = controller.GetSlotSkill(0);

            if (skill != null)
            {
                Debug.Log("Slot0技能:" + skill.SkillType);
            }
            else
            {
                Debug.Log("Slot0为空");
            }

        }

    }

}