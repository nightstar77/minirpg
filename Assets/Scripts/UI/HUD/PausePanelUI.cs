using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiniRPG
{
    /// <summary>
    /// 暂停面板 UI：游戏暂停时显示，提供继续/重新开始/返回主菜单按钮。
    /// 根据 GameManager 的 Paused 状态自动显隐。
    ///
    /// 重要：面板物体在 Hierarchy 中必须保持激活（m_IsActive 勾选为 true）。
    /// 隐藏改用 CanvasGroup（alpha=0 + 不接收射线），而不是 SetActive(false)。
    /// 原因：物体被 SetActive(false) 后本脚本的 Update 不会再执行，
    /// 面板将永远无法自行弹出（按 Esc 只会冻结游戏，面板不显示）。
    /// </summary>
    public class PausePanelUI : MonoBehaviour
    {
        [Header("按钮（可空，未拖则不显示对应功能）")]
        [Tooltip("继续游戏按钮")]
        public Button resumeButton;

        [Tooltip("重新开始按钮")]
        public Button restartButton;

        [Tooltip("返回主菜单按钮")]
        public Button mainMenuButton;

        [Tooltip("返回主菜单时加载的场景名")]
        public string mainMenuSceneName = "StartMenu";

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            // 用 CanvasGroup 控制显隐，保证 Update 始终运行
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            if (resumeButton != null) { resumeButton.onClick.AddListener(OnResume); }
            if (restartButton != null) { restartButton.onClick.AddListener(OnRestart); }
            if (mainMenuButton != null) { mainMenuButton.onClick.AddListener(OnMainMenu); }

            // 启动时隐藏
            SetVisible(false);
        }

        private void Update()
        {
            bool shouldShow = GameManager.Instance != null
                              && GameManager.Instance.State == GameManager.GameState.Paused;
            SetVisible(shouldShow);
        }

        /// <summary>
        /// 设置面板显隐（通过 CanvasGroup，不改变物体的激活状态）
        /// </summary>
        private void SetVisible(bool visible)
        {
            if (canvasGroup == null) { return; }

            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        private void OnResume()
        {
            GameManager.Instance.ResumeGame();
        }

        private void OnRestart()
        {
            // 恢复时间流速后重新加载当前场景
            GameManager.Instance.ResumeGame();
            LoadSceneSafely(SceneManager.GetActiveScene().name);
        }

        private void OnMainMenu()
        {
            GameManager.Instance.ResumeGame();
            LoadSceneSafely(mainMenuSceneName);
        }

        /// <summary>
        /// 安全切换场景：切换前清除 Editor 的选中对象。
        /// 原因：场景切换会销毁当前场景全部物体，如果 Inspector 此时仍预览被选中的面板/按钮，
        /// 会抛出 MissingReferenceException（LayoutPropertiesPreview.OnPreviewGUI 访问已销毁的 GameObject）。
        /// 该异常只发生在 Editor 中，清理选中即可避免。
        /// </summary>
        private void LoadSceneSafely(string sceneName)
        {
#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = null;
#endif
            SceneManager.LoadScene(sceneName);
        }
    }
}
