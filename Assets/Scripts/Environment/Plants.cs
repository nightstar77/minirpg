using UnityEngine;

/// <summary>
/// 场景植物装饰：玩家进入植物区域时半透明，离开时恢复，避免遮挡玩家
/// </summary>
public class Plants : MonoBehaviour
{
    public SpriteRenderer sr;

    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 玩家进入植物区域时，植物变透明
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(sr != null)
            {
                sr.color = new Color(1, 1, 1, 0.6f);
            }
        }
    }

    /// <summary>
    /// 玩家离开植物区域时，植物恢复原状
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (sr != null)
            {
                sr.color = new Color(1, 1, 1, 1);
            }
        }
    }
}
