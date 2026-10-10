using UnityEngine;

namespace NPC
{
    /// <summary>
    /// 玩家交互输入
    /// 设计思路参考 SkillInput.cs：使用旧 Input Manager（Input.GetKeyDown）
    /// 与项目现有输入风格保持一致
    /// 
    /// 工作流程：
    /// 1. NpcInteractCollider 检测到玩家进入范围 → 调用 SetCurrentInteractable(npc)
    /// 2. 玩家按交互键 → 调用当前 NPC 的 StartDialogue()
    /// 3. 对话中按交互键 → 调用 NextDialogueLine() 下一句
    /// 
    /// 使用方式：
    /// 挂在 Player 物体上，interactKey 可在 Inspector 调整
    /// 注意：当前交互键默认是 E，如果你技能键也用了 E，记得改一个
    /// </summary>
    public class InteractInput : MonoBehaviour
    {
        [Header("交互键设置")]
        [Tooltip("交互键（默认 E，如果和技能键冲突请改成其他键）")]
        public KeyCode interactKey = KeyCode.E;

        // 当前可交互的 NPC（由 NpcInteractCollider 设置/清除）
        private NpcBase currentInteractable;

        /// <summary>
        /// 设置当前可交互的 NPC
        /// 由 NpcInteractCollider 在玩家进入范围时调用
        /// </summary>
        public void SetCurrentInteractable(NpcBase npc)
        {
            currentInteractable = npc;
        }

        /// <summary>
        /// 清除当前可交互的 NPC（完全清除）
        /// </summary>
        public void ClearCurrentInteractable()
        {
            currentInteractable = null;
        }

        /// <summary>
        /// 清除指定的 NPC（只有当前交互对象匹配时才清除）
        /// 用于多 NPC 场景，防止离开一个 NPC 时误清除另一个 NPC 的引用
        /// </summary>
        public void ClearCurrentInteractable(NpcBase npc)
        {
            if (currentInteractable == npc)
            {
                currentInteractable = null;
            }
        }

        private void Update()
        {
            // 按下交互键
            if (Input.GetKeyDown(interactKey))
            {
                HandleInteractInput();
            }
        }

        /// <summary>
        /// 处理交互输入
        /// - 有可交互 NPC 且不在对话中 → 开始对话
        /// - 正在对话中 → 下一句
        /// - 没有可交互 NPC → 什么都不做
        /// </summary>
        private void HandleInteractInput()
        {
            if (currentInteractable == null) return;

            currentInteractable.StartDialogue();
        }
    }
}
