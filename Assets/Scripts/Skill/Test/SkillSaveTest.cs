using UnityEngine;

/// <summary>
/// 测试脚本：按Delete键清除技能存档
/// </summary>
public class SkillSaveTest : MonoBehaviour
{

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteKey("PlayerSkills");
            PlayerPrefs.Save();

            Debug.Log("删除技能存档");
        }

    }

}