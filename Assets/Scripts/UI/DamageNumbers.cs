using TMPro;
using UnityEngine;

namespace MiniRPG
{
    /// <summary>
    /// 伤害飘字：受击时在目标位置弹出伤害数字，上飘并淡出。
    /// 使用 TextMeshPro 世界空间文本，模板在运行时创建（无需手动制作预制体），
    /// 并复用 ObjectPool 缓存实例，避免频繁创建销毁。
    /// </summary>
    public static class DamageNumbers
    {
        private static GameObject template;

        /// <summary>
        /// 显示一个伤害数字
        /// </summary>
        /// <param name="worldPos">出现位置（一般为被击中的目标位置）</param>
        /// <param name="amount">伤害数值</param>
        /// <param name="color">颜色（玩家受伤用红色，敌人受伤用白色）</param>
        public static void Show(Vector3 worldPos, float amount, Color color)
        {
            // 伤害取整显示，避免出现 12.3456 这种长数字
            ShowText(worldPos, Mathf.RoundToInt(amount).ToString(), color);
        }

        /// <summary>
        /// 显示一段文字（如格挡提示）
        /// </summary>
        /// <param name="worldPos">出现位置</param>
        /// <param name="content">显示内容</param>
        /// <param name="color">文字颜色</param>
        public static void ShowText(Vector3 worldPos, string content, Color color)
        {
            // 项目没有配置 TMP 默认字体时直接跳过，避免每帧刷空引用警告
            if (TMP_Settings.defaultFontAsset == null) { return; }

            if (template == null)
            {
                template = CreateTemplate();
            }
            if (template == null) { return; }

            GameObject go = ObjectPool.Spawn(template, worldPos, Quaternion.identity);
            if (go == null) { return; }

            DamageNumberItem item = go.GetComponent<DamageNumberItem>();
            if (item != null)
            {
                item.Play(content, color);
            }
        }

        /// <summary>
        /// 运行时创建飘字模板（隐藏状态，仅作为池的生成源）
        /// </summary>
        private static GameObject CreateTemplate()
        {
            // 优先用 TMP 默认字体；未配置时回退到 TMP 内置的 LiberationSans SDF
            TMP_FontAsset font = TMP_Settings.defaultFontAsset;
            if (font == null)
            {
                font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            }
            if (font == null) { return null; }

            GameObject go = new GameObject("DamageNumberTemplate");

            TextMeshPro tmp = go.AddComponent<TextMeshPro>();
            tmp.font = font;
            tmp.fontSize = 4f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.sortingOrder = 200;
            tmp.text = "0";

            go.AddComponent<DamageNumberItem>();
            go.SetActive(false);
            return go;
        }
    }

    /// <summary>
    /// 单个伤害飘字实例：上飘 + 淡出，结束后归还对象池。
    /// </summary>
    public class DamageNumberItem : MonoBehaviour, IPoolable
    {
        [SerializeField] private float lifetime = 0.7f;
        [SerializeField] private float riseSpeed = 1.2f;

        private TextMeshPro textMesh;
        private float elapsed;
        private Color baseColor = Color.white;

        private void Awake()
        {
            textMesh = GetComponent<TextMeshPro>();
        }

        /// <summary>
        /// 设置显示内容并开始播放
        /// </summary>
        public void Play(string content, Color color)
        {
            if (textMesh == null) { return; }

            textMesh.text = content;
            textMesh.color = color;
            baseColor = color;
            elapsed = 0f;

            // 轻微随机偏移，避免同时多个数字完全重叠
            transform.position += new Vector3(
                Random.Range(-0.15f, 0.15f),
                Random.Range(0f, 0.15f),
                0f);
        }

        public void OnSpawned()
        {
            elapsed = 0f;
        }

        public void OnDespawned()
        {
        }

        private void Update()
        {
            if (textMesh == null)
            {
                ObjectPool.Despawn(gameObject);
                return;
            }

            elapsed += Time.deltaTime;
            transform.position += Vector3.up * (riseSpeed * Time.deltaTime);

            float progress = elapsed / lifetime;
            if (progress >= 1f)
            {
                ObjectPool.Despawn(gameObject);
                return;
            }

            Color c = baseColor;
            c.a = 1f - progress;
            textMesh.color = c;
        }
    }
}
