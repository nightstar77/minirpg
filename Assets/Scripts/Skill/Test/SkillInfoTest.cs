using UnityEngine;

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