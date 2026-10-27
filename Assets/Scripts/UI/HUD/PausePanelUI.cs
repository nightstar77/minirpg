using UnityEngine;

namespace MiniRPG.Core
{
    /// <summary>
    /// 暂停面板 UI：游戏暂停时显示，提供继续/重新开始/返回主菜单按钮。
    /// 订阅 GameManager 状态变化自动显隐。
    ///
    /// Unity 搭建：
    /// - Canvas 下建一个全屏半透明遮罩 + 面板
    /// - 三个按钮：继续游戏、重新开始、返回主菜单
    /// - 把按钮拖到 Inspector 的 resumeButton/restartButton/mainMenuButton
    /// - 面板默认隐藏（脚本 Awake 中 SetActive(false)）
    /// </summary>
    public class PausePanelUI : MonoBehaviour
    {
        [Header("按钮（可空，未拖则不显示对应功能）")]
        [Tooltip("继续游戏按钮")]
        public UnityEngine.UI.Button resumeButton;

        [Tooltip("重新开始按钮")]
        public UnityEngine.UI.Button restartButton;

        [Tooltip("返回主菜单按钮")]
        public UnityEngine.UI.Button mainMenuButton;

        private void Awake()
        {
            if (resumeButton != null) { resumeButton.onClick.AddListener(OnResume); }
            if (restartButton != null) { restartButton.onClick.AddListener(OnRestart); }
            if (mainMenuButton != null) { mainMenuButton.onClick.AddListener(OnMainMenu); }

            // 启动时隐藏
            gameObject.SetActive(false);
        }

        private void Update()
        {
            // 根据 GameManager 状态显隐面板
            bool shouldShow = GameManager.Instance != null
                              && GameManager.Instance.State == GameManager.GameState.Paused;
            if (gameObject.activeSelf != shouldShow)
            {
                gameObject.SetActive(shouldShow);
            }
        }

        private void OnResume()
        {
            GameManager.Instance.ResumeGame();
        }

        private void OnRestart()
        {
            // 恢复时间流速后重新加载当前场景
            GameManager.Instance.ResumeGame();
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        private void OnMainMenu()
        {
            GameManager.Instance.ResumeGame();
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartMenu");
        }
    }
}
