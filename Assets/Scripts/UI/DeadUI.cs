using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 死亡界面：点击重新开始按钮重载游戏场景
/// </summary>
public class DeadUI : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
