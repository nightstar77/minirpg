using UnityEngine;

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