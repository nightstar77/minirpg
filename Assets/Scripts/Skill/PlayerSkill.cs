using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    #region ===== Component =====

    private DashSkill dashSkill;

    #endregion

    #region ===== Unity =====

    private void Awake()
    {
        dashSkill = GetComponent<DashSkill>();
    }

    private void Update()
    {
        ReadSkillInput();
    }

    #endregion

    #region ===== Input =====

    private void ReadSkillInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (dashSkill.CanUse())
            {
                dashSkill.Use();
            }
        }
    }

    #endregion
}