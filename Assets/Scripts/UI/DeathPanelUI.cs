using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 玩家死亡面板：玩家死亡后弹出，显示提示文本和重新开始按钮。
///
/// 使用方式：
/// 1. 在 Canvas 下建一个死亡面板（背景 + 文本 + 按钮）
/// 2. 给面板根物体挂此脚本
/// 3. 拖拽赋值：deathText、restartButton
/// 4. 确保场景里有 PlayerHealth（会自动查找玩家身上的）
/// </summary>
public class DeathPanelUI : MonoBehaviour
{
    [Header("UI 引用")]
    [Tooltip("死亡提示文本（如'你死了'）")]
    [SerializeField] private TextMeshProUGUI deathText;

    [Tooltip("重新开始按钮")]
    [SerializeField] private Button restartButton;

    [Header("设置")]
    [Tooltip("死亡后显示的文本内容")]
    [SerializeField] private string deathMessage = "你死了";

    private PlayerHealth playerHealth;

    private void Awake()
    {
        // 先完成所有初始化（此时物体还是激活状态，Start 不会在禁用后执行）
        // 查找玩家身上的 PlayerHealth
        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            playerHealth = player;
            playerHealth.OnPlayerDead += OnPlayerDead;
        }
        else
        {
            Debug.LogWarning("DeathPanelUI: 场景中没有找到 PlayerHealth");
        }

        // 绑定重新开始按钮
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        // 最后隐藏面板
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDead -= OnPlayerDead;
        }
    }

    /// <summary>
    /// 玩家死亡回调：显示死亡面板
    /// </summary>
    private void OnPlayerDead()
    {
        gameObject.SetActive(true);

        if (deathText != null)
        {
            deathText.text = deathMessage;
        }
    }

    /// <summary>
    /// 点击重新开始按钮：重新加载当前场景
    /// </summary>
    private void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
