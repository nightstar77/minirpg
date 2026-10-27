using UnityEngine;

namespace NPC
{
    /// <summary>
    /// NPC 交互范围触发器
    /// 设计思路完全参考 EnemyPursuitCollider：
    /// - 挂在 NPC 的子物体上（带 CircleCollider2D 或 BoxCollider2D，勾选 Is Trigger）
    /// - 只负责检测玩家进入/离开，逻辑全部回调给 NpcBase
    /// - 触发器与业务逻辑分离，职责单一
    /// 
    /// 使用方式：
    /// 1. 在 NPC 下新建子物体，命名为 "InteractRange"
    /// 2. 添加 Collider2D（2D 触发器），勾选 Is Trigger，调整大小
    /// 3. 挂上此脚本，拖拽 npcScript 引用
    /// 4. 确保玩家身上有 "Player" tag 和 Collider2D
    /// </summary>
    public class NpcInteractCollider : MonoBehaviour
    {
        [Header("宿主引用（拖拽赋值）")]
        [Tooltip("交互范围所属的 NPC 主体")]
        public NpcBase npcScript;

        /// <summary>
        /// 玩家进入交互范围
        /// 同时通知 NPC 主体（显示提示）和玩家输入组件（记录当前交互对象）
        /// </summary>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            // 通知 NPC 主体：玩家进入范围
            npcScript?.PlayerEnterInteractRange(collision.transform);

            // 通知玩家输入组件：当前可以交互的 NPC 是我
            InteractInput input = collision.GetComponent<InteractInput>();
            input?.SetCurrentInteractable(npcScript);
        }

        /// <summary>
        /// 玩家离开交互范围
        /// 同时通知 NPC 主体（隐藏提示）和玩家输入组件（清除当前交互对象）
        /// </summary>
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            // 通知 NPC 主体：玩家离开范围
            npcScript?.PlayerExitInteractRange();

            // 通知玩家输入组件：清除当前交互对象
            // 只有当前交互对象是这个 NPC 时才清除，避免多 NPC 距离近时误清除
            InteractInput input = collision.GetComponent<InteractInput>();
            if (input != null)
            {
                input.ClearCurrentInteractable(npcScript);
            }
        }
    }
}
