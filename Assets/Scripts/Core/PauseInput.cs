using UnityEngine;

namespace MiniRPG.Core
{
    /// <summary>
    /// 暂停输入：监听 Esc 键切换游戏暂停状态。
    /// 挂在任意场景物体上（建议挂在 HUD 容器或 GameManager 同级物体）。
    /// 改键捕获期间不响应暂停，避免冲突。
    /// </summary>
    public class PauseInput : MonoBehaviour
    {
        [Tooltip("暂停按键，默认 Esc")]
        public KeyCode pauseKey = KeyCode.Escape;

        private void Update()
        {
            if (GameInputs.IsCapturing) return;

            if (Input.GetKeyDown(pauseKey))
            {
                // 打开设置面板时不切换暂停（设置面板自带暂停逻辑可选）
                if (SettingsPanelUI.Instance != null && SettingsPanelUI.Instance.gameObject.activeSelf)
                {
                    return;
                }
                GameManager.Instance.TogglePause();
            }
        }
    }
}
