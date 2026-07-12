using UnityEngine;

public class EnemypursuitCollider : MonoBehaviour
{
    public EnemyBase enemyScript;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {   //如果玩家进入追击范围，则调用EnemyBase的PlayerEnterPursuitBox方法
            enemyScript.PlayerEnterPursuitBox(collision.GetComponent<Player>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {   //如果玩家退出追击范围，则调用EnemyBase的PlayerExitPursuitBox方法
            enemyScript.PlayerExitPursuitBox(collision.GetComponent<Player>());
        }
    }
}
