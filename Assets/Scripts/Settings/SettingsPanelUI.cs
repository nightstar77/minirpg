using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏内设置面板：两个页签
/// 1. 按键设置页（默认）：改技能/攻击/防御按键
/// 2. 音量设置页：BGM 与音效音量调节
///
/// Unity 搭建：
/// - HUD 上的齿轮按钮 → onClick 拖到本组件的 Toggle()
/// - 页签按钮（齿轮/音乐图标）→ onClick 拖到 ShowKeybindTab()/ShowVolumeTab()
/// - 关闭按钮 → onClick 拖到 Close()
/// - 面板保持激活，脚本启动时自动隐藏
/// </summary>
public class SettingsPanelUI : MonoBehaviour
{
    public static SettingsPanelUI Instance { get; private set; }

    [Header("页签页面")]
    [Tooltip("按键设置页（默认显示）")]
    [SerializeField] private GameObject keybindPage;

    [Tooltip("音量设置页")]
    [SerializeField] private GameObject volumePage;

    [Header("按钮")]
    [Tooltip("按键设置页签按钮（齿轮图标）")]
    [SerializeField] private Button keybindTabButton;

    [Tooltip("音量设置页签按钮（音乐图标）")]
    [SerializeField] private Button volumeTabButton;

    [Tooltip("关闭按钮")]
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        // 单例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (keybindTabButton != null) { keybindTabButton.onClick.AddListener(ShowKeybindTab); }
        if (volumeTabButton != null) { volumeTabButton.onClick.AddListener(ShowVolumeTab); }
        if (closeButton != null) { closeButton.onClick.AddListener(Close); }

        // 启动时隐藏面板
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #region ===== 开关 =====

    /// <summary>
    /// HUD 齿轮按钮调用：开则关、关则开
    /// </summary>
    public void Toggle()
    {
        if (gameObject.activeSelf)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
        ShowKeybindTab(); // 每次打开默认显示按键设置页
    }

    public void Close()
    {
        // 关闭面板时取消未完成的改键捕获
        GameInputs.EndCapture();
        gameObject.SetActive(false);
    }

    #endregion

    #region ===== 页签 =====

    public void ShowKeybindTab()
    {
        if (keybindPage != null) { keybindPage.SetActive(true); }
        if (volumePage != null) { volumePage.SetActive(false); }
    }

    public void ShowVolumeTab()
    {
        if (keybindPage != null) { keybindPage.SetActive(false); }
        if (volumePage != null) { volumePage.SetActive(true); }
    }

    #endregion
}
