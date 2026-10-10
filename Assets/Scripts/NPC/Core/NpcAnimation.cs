using UnityEngine;

namespace NPC
{
    /// <summary>
    /// NPC 动画控制器
    /// 设计思路参考 EnemyAnimation：把 Animator 操作封装起来，
    /// NpcBase 不直接操作 Animator，全部通过此类控制，保持职责单一
    /// NPC 目前只有站立（Idle）和对话两种状态，对话时可能播特殊说话动画
    /// </summary>
    public class NpcAnimation : MonoBehaviour
    {
        [Header("组件引用（拖拽赋值）")]
        [Tooltip("NPC 的 Animator 组件")]
        public Animator animator;

        [Tooltip("NPC 的 SpriteRenderer，用于左右翻转")]
        public SpriteRenderer spriteRenderer;

        // Animator 参数名（和 EnemyAnimation 命名风格保持一致）
        private static readonly int IsTalking = Animator.StringToHash("IsTalking");

        /// <summary>
        /// 播放站立（空闲）动画
        /// </summary>
        public void PlayIdle()
        {
            if (animator == null) return;
            animator.SetBool(IsTalking, false);
        }

        /// <summary>
        /// 播放对话（说话）动画
        /// </summary>
        public void PlayTalking()
        {
            if (animator == null) return;
            animator.SetBool(IsTalking, true);
        }

        /// <summary>
        /// 设置 NPC 朝向（翻转 SpriteRenderer）
        /// </summary>
        /// <param name="faceRight">true=朝右，false=朝左</param>
        public void SetFacing(bool faceRight)
        {
            if (spriteRenderer == null) return;
            spriteRenderer.flipX = !faceRight;
        }

        /// <summary>
        /// 让 NPC 面向指定的 Transform
        /// 对话开始时调用，让 NPC 面向玩家
        /// </summary>
        public void FaceTarget(Transform target)
        {
            if (target == null || spriteRenderer == null) return;
            bool faceRight = target.position.x > transform.position.x;
            spriteRenderer.flipX = !faceRight;
        }
    }
}
