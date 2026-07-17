using UnityEngine;

public class HitBoxTest : MonoBehaviour
{
    public WeaponHitBox hitBox;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hitBox.EnableHitBox();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            hitBox.DisableHitBox();
        }
    }

}