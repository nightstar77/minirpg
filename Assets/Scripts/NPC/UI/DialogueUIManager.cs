using NPC.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NPC.UI
{
    /// <summary>
    /// 对话框 UI 管理器（全局单例）
    /// 
    /// 设计说明：
    /// 场景里只有一个对话框，所有 NPC 共用。NPC 在对话开始/行切换/结束时
    /// 直接调用此类的公共方法来更新 UI，比事件订阅更直接。
    /// 
    /// 使用方式：
    /// 1. 在场景 Canvas 下建对话框 UI
    /// 2. 挂上此脚本，拖拽各组件引用
    /// 3. NPC 脚本通过 DialogueUIManager.Instance 调用 Show/UpdateLine/Hide
    /// </summary>
    public class DialogueUIManager : MonoBehaviour
    {
        // 全局单例（简单实现，场景里只能有一个 DialogueUIManager）
        public static DialogueUIManager Instance { get; private set; }

        [Header("对话框组件（拖拽赋值）")]
        [Tooltip("对话框根物体（显示/隐藏用）")]
        public GameObject dialoguePanel;

        [Tooltip("NPC 名称文本")]
        public TextMeshProUGUI nameText;

        [Tooltip("对话内容文本")]
        public TextMeshProUGUI dialogueText;

        [Tooltip("NPC 头像（可选，没有就留空）")]
        public Image portraitImage;

        [Tooltip("继续提示图标（比如「点击继续」的小箭头）")]
        public GameObject continuePrompt;

        [Header("继续提示动画")]
        [Tooltip("继续提示上下浮动的速度")]
        public float floatSpeed = 2f;

        [Tooltip("继续提示上下浮动的幅度")]
        public float floatAmount = 5f;

        // 继续提示的初始位置（浮动动画用）
        private Vector2 continuePromptStartPos;

        private void Awake()
        {
            // 单例注册
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // 初始隐藏对话框
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            // 记录继续提示的初始位置
            if (continuePrompt != null)
            {
                RectTransform rt = continuePrompt.GetComponent<RectTransform>();
                if (rt != null)
                {
                    continuePromptStartPos = rt.anchoredPosition;
                }
            }
        }

        private void OnDestroy()
        {
            // 单例注销
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            // 继续提示图标上下浮动效果
            UpdateContinuePromptFloat();
        }

        #region ===== 公共 API（由 NpcBase 调用） =====

        /// <summary>
        /// 显示对话框
        /// 对话开始时由 NpcBase 调用
        /// </summary>
        /// <param name="data">对话数据（包含 NPC 名称、头像、对话行等）</param>
        public void ShowDialogue(NpcDialogueData data)
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }

            // 设置 NPC 名称
            if (nameText != null && data != null)
            {
                nameText.text = data.npcName;
            }

            // 设置头像（有就显示，没有就隐藏头像框）
            if (portraitImage != null)
            {
                if (data != null && data.npcPortrait != null)
                {
                    portraitImage.gameObject.SetActive(true);
                    portraitImage.sprite = data.npcPortrait;
                }
                else
                {
                    portraitImage.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 更新当前对话行内容
        /// 切换到下一句时由 NpcBase 调用
        /// </summary>
        /// <param name="line">当前行文本</param>
        /// <param name="lineIndex">当前行号（从 0 开始）</param>
        /// <param name="totalLines">总行数</param>
        public void UpdateLine(string line, int lineIndex, int totalLines)
        {
            if (dialogueText != null)
            {
                dialogueText.text = line;
            }

            // 最后一行时隐藏继续提示（表示对话即将结束）
            if (continuePrompt != null)
            {
                bool isLastLine = lineIndex >= totalLines - 1;
                continuePrompt.SetActive(!isLastLine);
            }
        }

        /// <summary>
        /// 隐藏对话框
        /// 对话结束时由 NpcBase 调用
        /// </summary>
        public void HideDialogue()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            // 重置继续提示位置
            if (continuePrompt != null)
            {
                RectTransform rt = continuePrompt.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchoredPosition = continuePromptStartPos;
                }
            }
        }

        #endregion

        #region ===== 继续提示浮动动画 =====

        /// <summary>
        /// 继续提示图标上下浮动效果
        /// 纯正弦波动，不需要 DOTween 等第三方插件
        /// </summary>
        private void UpdateContinuePromptFloat()
        {
            if (continuePrompt == null || !continuePrompt.activeSelf) return;

            RectTransform rt = continuePrompt.GetComponent<RectTransform>();
            if (rt == null) return;

            float offset = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
            rt.anchoredPosition = continuePromptStartPos + new Vector2(0, offset);
        }

        #endregion
    }
}
