using UnityEngine;

namespace MiniRPG
{
    /// <summary>
    /// 屏幕震动：计算当前帧的震动偏移量，由 CameraFollow 叠加到跟随位置上。
    /// 本脚本不再直接修改 transform.position，避免与摄像机跟随冲突。
    /// 使用 unscaledDeltaTime，因此命中停顿（Time.timeScale 接近 0）期间依然生效。
    ///
    /// 用法：把本组件挂到 Main Camera，其他系统调用 CameraShake.ShakeCamera(...)。
    /// 未挂载时所有调用安全失效（不会报错）。
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }

        [Header("震动幅度上限")]
        [Tooltip("单次震动最大位移（世界单位），避免数值过大导致镜头偏移明显")]
        public float maxMagnitude = 0.25f;

        /// <summary>当前帧的震动偏移量，由 CameraFollow 叠加到最终位置</summary>
        public Vector3 ShakeOffset { get; private set; }

        private float remainingTime;
        private float magnitude;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// 触发一次震动（持续时间单位秒，幅度单位世界单位）
        /// </summary>
        public static void ShakeCamera(float duration, float magnitude)
        {
            if (Instance == null) { return; }
            Instance.ShakeInternal(duration, magnitude);
        }

        private void ShakeInternal(float duration, float magnitudeValue)
        {
            // 短震动直接覆盖，长震动取更强的一次
            remainingTime = Mathf.Max(remainingTime, duration);
            this.magnitude = Mathf.Max(this.magnitude, Mathf.Min(magnitudeValue, maxMagnitude));
        }

        private void Update()
        {
            if (remainingTime <= 0f)
            {
                ShakeOffset = Vector3.zero;
                magnitude = 0f;
                return;
            }

            remainingTime -= Time.unscaledDeltaTime;

            Vector2 offset = Random.insideUnitCircle * magnitude;
            ShakeOffset = new Vector3(offset.x, offset.y, 0f);

            // 随时间线性衰减
            magnitude = Mathf.Max(0f, magnitude - Time.unscaledDeltaTime * magnitude * 4f);
        }
    }
}
