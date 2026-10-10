using NPC.Data;
using NPC.UI;
using UnityEngine;

namespace NPC
{
    /// <summary>
    /// NPC 状态枚举
    /// 参考 EnemyState 的设计：NPC 有站立和对话两种核心状态
    /// </summary>
    public enum NpcState
    {
        idle,       // 站立/空闲
        dialogue    // 对话中
    }

    /// <summary>
    /// NPC 基类（有限状态机）
    /// 设计思路完全参考 EnemyBase：
    /// - 枚举驱动的 FSM（Enter/Update/Exit 三段式）
    /// - 触发器检测由子物体 NpcInteractCollider 负责，回调到本类
    /// - 动画通过 NpcAnimation 组件控制，不直接操作 Animator
    /// - 对话数据通过 NpcDialogueData（ScriptableObject）配置
    /// - UI 通过 DialogueUIManager 单例直接调用显示
    /// 
    /// 使用方式：
    /// 1. 把此脚本挂到 NPC 物体上
    /// 2. 在 Inspector 拖拽 npcAnimation、dialogueData、interactPrompt 等引用
    /// 3. 子物体放一个带 Collider2D + NpcInteractCollider 的交互检测区
    /// 4. 场景里要有 DialogueUIManager（对话框 UI）
    /// </summary>
    public class NpcBase : MonoBehaviour
    {
        [Header("核心组件（拖拽赋值）")]
        [Tooltip("NPC 动画控制器")]
        public NpcAnimation npcAnimation;

        [Tooltip("对话数据（ScriptableObject），右键 Create → NPC → Npc Dialogue Data 创建")]
        public NpcDialogueData dialogueData;

        [Header("交互提示（拖拽赋值）")]
        [Tooltip("玩家靠近时显示的按键提示图标（NPC 头顶上方）")]
        public GameObject interactPrompt;

        // 当前状态
        private NpcState state;
        // 上一个状态（保留字段，未来扩展用）
        private NpcState stateOld;

        // 玩家是否在交互范围内（由 NpcInteractCollider 维护）
        private bool isPlayerInRange;
        // 当前交互的玩家 Transform
        private Transform playerTransform;

        // 当前对话行索引
        private int currentDialogueIndex;

        /// <summary>
        /// 玩家是否在交互范围内（只读）
        /// </summary>
        public bool IsPlayerInRange => isPlayerInRange;

        private void Start()
        {
            // 默认进入 idle 状态
            ChangeState(NpcState.idle);

            // 初始隐藏交互提示
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }

        private void Update()
        {
            // 按当前状态分发 Update
            switch (state)
            {
                case NpcState.idle:
                    IdleUpdate();
                    break;
                case NpcState.dialogue:
                    DialogueUpdate();
                    break;
            }
        }

        #region ===== 状态切换核心 =====

        /// <summary>
        /// 切换状态（参考 EnemyBase.ChangeState）
        /// 负责调用旧状态的 Exit 和新状态的 Enter
        /// </summary>
        private void ChangeState(NpcState newState)
        {
            // 相同状态不重复切换
            if (state == newState) return;

            // 旧状态退出
            switch (state)
            {
                case NpcState.idle:
                    IdleExit();
                    break;
                case NpcState.dialogue:
                    DialogueExit();
                    break;
            }

            stateOld = state;
            state = newState;

            // 新状态进入
            switch (state)
            {
                case NpcState.idle:
                    IdleEnter();
                    break;
                case NpcState.dialogue:
                    DialogueEnter();
                    break;
            }
        }

        #endregion

        #region ===== Idle 状态（站立） =====

        private void IdleEnter()
        {
            // 播站立动画
            if (npcAnimation != null)
            {
                npcAnimation.PlayIdle();
            }
        }

        private void IdleUpdate()
        {
            // idle 状态下每帧不需要做什么
            // 交互检测由 NpcInteractCollider 触发
        }

        private void IdleExit()
        {
            // 离开 idle 状态时的清理（目前不需要）
        }

        #endregion

        #region ===== Dialogue 状态（对话中） =====

        private void DialogueEnter()
        {
            // 对话开始，重置行索引
            currentDialogueIndex = 0;

            // 让 NPC 面向玩家
            if (npcAnimation != null && playerTransform != null)
            {
                npcAnimation.FaceTarget(playerTransform);
                npcAnimation.PlayTalking();
            }

            // 隐藏交互提示（对话中不显示）
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }

            // 显示对话框 UI（通过单例直接调用）
            if (DialogueUIManager.Instance != null && dialogueData != null)
            {
                DialogueUIManager.Instance.ShowDialogue(dialogueData);

                // 显示第一行对话
                if (dialogueData.dialogueLines != null && dialogueData.dialogueLines.Length > 0)
                {
                    DialogueUIManager.Instance.UpdateLine(
                        dialogueData.dialogueLines[0],
                        0,
                        dialogueData.dialogueLines.Length
                    );
                }
            }
        }

        private void DialogueUpdate()
        {
            // 对话中的每帧逻辑由输入驱动（玩家按键 → NextDialogueLine）
            // 这里只做状态维护
        }

        private void DialogueExit()
        {
            // 对话结束，回到站立动画
            if (npcAnimation != null)
            {
                npcAnimation.PlayIdle();
            }

            // 隐藏对话框 UI
            if (DialogueUIManager.Instance != null)
            {
                DialogueUIManager.Instance.HideDialogue();
            }

            // 如果玩家还在范围内，重新显示交互提示
            if (isPlayerInRange && interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }

            // 如果对话数据配置了结束事件触发，可在此处扩展任务系统
            // if (dialogueData != null && dialogueData.triggerEventOnEnd) { ... }
        }

        #endregion

        #region ===== 交互检测回调（由 NpcInteractCollider 调用） =====

        /// <summary>
        /// 玩家进入交互范围
        /// 由 NpcInteractCollider.OnTriggerEnter2D 调用
        /// </summary>
        public virtual void PlayerEnterInteractRange(Transform player)
        {
            isPlayerInRange = true;
            playerTransform = player;

            // 只有 idle 状态才显示交互提示（对话中不重复显示）
            if (state == NpcState.idle && interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }
        }

        /// <summary>
        /// 玩家离开交互范围
        /// 由 NpcInteractCollider.OnTriggerExit2D 调用
        /// </summary>
        public virtual void PlayerExitInteractRange()
        {
            isPlayerInRange = false;

            // 隐藏交互提示
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }

            // 如果玩家在对话中走出范围，强制结束对话
            if (state == NpcState.dialogue)
            {
                EndDialogue();
            }

            playerTransform = null;
        }

        #endregion

        #region ===== 对话控制（由 InteractInput 调用） =====

        /// <summary>
        /// 开始对话 / 下一句
        /// 玩家按交互键时调用：
        /// - idle 状态 → 开始对话
        /// - dialogue 状态 → 下一句（最后一句则结束）
        /// </summary>
        public void StartDialogue()
        {
            // 不在范围内或没有对话数据，不处理
            if (!isPlayerInRange || dialogueData == null) return;

            // 已经在对话中 → 走"下一句"逻辑
            if (state == NpcState.dialogue)
            {
                NextDialogueLine();
                return;
            }

            // idle 状态 → 开始对话
            ChangeState(NpcState.dialogue);
        }

        /// <summary>
        /// 切换到下一句对话
        /// </summary>
        public void NextDialogueLine()
        {
            if (state != NpcState.dialogue || dialogueData == null ||
                dialogueData.dialogueLines == null) return;

            currentDialogueIndex++;

            // 超过最后一行 → 结束对话
            if (currentDialogueIndex >= dialogueData.dialogueLines.Length)
            {
                EndDialogue();
                return;
            }

            // 更新 UI 显示
            if (DialogueUIManager.Instance != null)
            {
                DialogueUIManager.Instance.UpdateLine(
                    dialogueData.dialogueLines[currentDialogueIndex],
                    currentDialogueIndex,
                    dialogueData.dialogueLines.Length
                );
            }
        }

        /// <summary>
        /// 强制结束对话
        /// </summary>
        public void EndDialogue()
        {
            if (state != NpcState.dialogue) return;
            ChangeState(NpcState.idle);
        }

        /// <summary>
        /// 替换对话数据（用于任务进度变化时切换对话内容）
        /// 调用后 NPC 下一次对话就会用新数据
        /// </summary>
        public void SetDialogueData(NpcDialogueData newData)
        {
            dialogueData = newData;
        }

        #endregion
    }
}
