using UnityEngine;

namespace NPC.UI
{
    /// <summary>
    /// 交互提示图标跟随脚本
    /// 
    /// 问题背景：
    /// InteractPrompt 是 UI 元素，必须在 Canvas 下才能渲染。
    /// 但 NPC 在世界坐标里，两者坐标系不同。
    /// 此脚本把 NPC 的世界坐标转换成屏幕坐标，让 UI 图标跟随 NPC 头顶显示。
    /// 
    /// 使用方式：
    /// 1. 把 InteractPrompt 放在 Canvas 下面
    /// 2. 挂上此脚本
    /// 3. 拖拽 target（NPC 物体）和 mainCamera（主相机）
    /// 4. 调整 offsetY 控制图标在 NPC 头顶上方多少距离
    /// 
    /// 注意：
    /// - 如果是多个 NPC，每个 NPC 对应一个 InteractPrompt 实例
    /// - 也可以把 InteractPrompt 做成预制体，NPC 上动态实例化
    /// </summary>
    public class InteractPromptFollow : MonoBehaviour
    {
        [Header("跟随目标（拖拽赋值）")]
        [Tooltip("要跟随的 NPC 物体")]
        public Transform target;

        [Tooltip("主相机，用于世界坐标转屏幕坐标。不填则自动找 Camera.main")]
        public Camera mainCamera;

        [Header("位置偏移")]
        [Tooltip("头顶上方的偏移量（世界单位），正值=向上")]
        public float offsetY = 1.5f;

        [Tooltip("屏幕坐标的额外像素偏移（向上）")]
        public float pixelOffsetY = 50f;

        // 缓存的 RectTransform
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            // 没指定相机就自动找
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void LateUpdate()
        {
            // 目标或相机不存在就不更新
            if (target == null || mainCamera == null) return;

            // 计算 NPC 头顶的世界坐标
            Vector3 worldPos = target.position + new Vector3(0, offsetY, 0);

            // 世界坐标 → 屏幕坐标
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

            // 屏幕坐标 → Canvas 中的 anchoredPosition
            // Screen Space - Overlay 模式下，屏幕坐标就是 Canvas 坐标
            // 加一个像素偏移让图标在头顶上方一点
            if (rectTransform != null)
            {
                rectTransform.position = screenPos + new Vector3(0, pixelOffsetY, 0);
            }
            else
            {
                transform.position = screenPos + new Vector3(0, pixelOffsetY, 0);
            }
        }

        /// <summary>
        /// 设置跟随目标（运行时动态切换 NPC 时用）
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
