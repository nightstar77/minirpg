using UnityEngine;
using UnityEngine.InputSystem;


public class SkillInput : MonoBehaviour
{

    private SkillController skillController;


    private void Awake()
    {
        skillController = GetComponent<SkillController>();
    }


    private void Update()
    {

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            skillController.UseSkill(0);
        }


        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            skillController.UseSkill(1);
        }

    }

}