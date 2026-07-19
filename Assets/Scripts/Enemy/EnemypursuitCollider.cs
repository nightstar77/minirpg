using UnityEngine;

public class EnemypursuitCollider : MonoBehaviour
{
    public EnemyBase enemyScript;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        enemyScript.PlayerEnterPursuitBox(collision.transform);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        enemyScript.PlayerExitPursuitBox();
    }
}
