using UnityEngine;

public class SkillEquipTest : MonoBehaviour
{
    private SkillEquipManager equipManager;

    private void Awake()
    {
        equipManager = GetComponent<SkillEquipManager>();
    }

    private void Update()
    {
        //测试装备火球到0号槽
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            equipManager.EquipSkill(SkillType.Fireball, 0);
        }

        //测试装备冲刺到1号槽
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            equipManager.EquipSkill(SkillType.Dash, 1);
        }

    }

}