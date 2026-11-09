using UnityEngine;

namespace MiniRPG
{
    /// <summary>
    /// 摄像机跟随：平滑跟随目标（玩家），可选边界 clamp。
    /// 挂在主摄像机上，Inspector 中拖入目标或留空自动按 Player Tag 查找。
    /// 使用 LateUpdate 确保在目标移动后再跟随，避免抖动。
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("跟随目标")]
        [Tooltip("跟随的目标 Transform，留空则自动查找 Tag 为 Player 的物体")]
        [SerializeField] private Transform target;

        [Header("跟随参数")]
        [SerializeField] private float followSpeed = 8f;
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        [Header("边界限制")]
        [Tooltip("是否启用摄像机边界限制")]
        [SerializeField] private bool clampToBounds = false;
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        private void Awake()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
                else
                {
                    Logger.LogWarning("CameraFollow: 未找到 Player 目标，请在 Inspector 中指定");
                }
            }
        }

        private void LateUpdate()
        {
            if (target == null) { return; }

            Vector3 desired = target.position + offset;
            Vector3 smoothed = Vector3.Lerp(transform.position, desired, followSpeed * Time.deltaTime);

            if (clampToBounds)
            {
                smoothed.x = Mathf.Clamp(smoothed.x, minBounds.x, maxBounds.x);
                smoothed.y = Mathf.Clamp(smoothed.y, minBounds.y, maxBounds.y);
            }

            // 叠加屏幕震动偏移（CameraShake 在 Update 中只算偏移、不写 transform，
            // 此处统一应用，避免两个脚本争抢 transform 导致跟随失效）
            Vector3 shake = CameraShake.Instance != null ? CameraShake.Instance.ShakeOffset : Vector3.zero;
            transform.position = smoothed + shake;
        }
    }
}
