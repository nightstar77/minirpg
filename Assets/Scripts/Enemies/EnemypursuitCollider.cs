using UnityEngine;

/// <summary>
/// 敌人警戒范围触发器：检测玩家进入/离开警戒区域，通知 EnemyBase 切换追击/巡逻状态
/// </summary>
public class EnemyPursuitCollider : MonoBehaviour
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
