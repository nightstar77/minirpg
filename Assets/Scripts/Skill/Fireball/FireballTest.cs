using UnityEngine;


public class FireballTest : MonoBehaviour
{

    public FireballSkill fireballSkill;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            fireballSkill.Use();
        }
    }
}