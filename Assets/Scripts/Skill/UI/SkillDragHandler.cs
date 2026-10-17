using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// 通用技能拖拽处理器：可挂在技能树节点、技能槽等任意技能图标上。
/// 使用 SkillDragPayload 传递数据，拖拽图标通过预制体实例化，可配置性更强。
/// 带有弹性放大、回弹、透明度渐变等动画效果。
/// canDrag 由外部控制（如技能树节点根据解锁状态、技能槽根据是否有技能）。
/// </summary>
public class SkillDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("拖拽设置")]
    [Tooltip("拖拽图标相对鼠标的偏移量（像素），避免遮挡光标")]
    public Vector2 dragOffset = new Vector2(30f, -30f);

    [Tooltip("拖拽时原图标透明度")]
    [Range(0f, 1f)]
    public float dragAlpha = 0.4f;

    [Header("拖拽图标预制体")]
    [Tooltip("拖拽时跟随鼠标的图标预制体。为空则运行时动态创建（兼容旧版）。")]
    public GameObject dragIconPrefab;

    [Header("动画设置")]
    [Tooltip("拖拽图标放大倍数（拿起时弹到这么大，然后回落到 1）")]
    public float popScale = 1.25f;

    [Tooltip("放大动画时长（秒）")]
    public float popDuration = 0.15f;

    [Tooltip("拖到空白处回弹动画时长（秒）")]
    public float snapBackDuration = 0.2f;

    [Tooltip("透明度渐变时长（秒）")]
    public float fadeDuration = 0.1f;

    // 拖拽数据包，接收方从这里读取信息，避免直接耦合
    private SkillDragPayload payload;
    // 原图标 Image 引用，用于设置透明度
    private Image icon;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    // 跟随鼠标的拖拽图标
    private GameObject dragIcon;
    // 拖拽图标的 RectTransform 缓存
    private RectTransform dragIconRect;
    // 拖拽图标的 Image 组件缓存
    private Image dragIconImage;

    // 正在运行的协程引用，用于取消
    private Coroutine currentAnimCoroutine;

    /// <summary>
    /// 拖拽开始事件，携带本次拖拽的数据
    /// 外部系统（如音效、全局拖拽管理器）可订阅
    /// </summary>
    public event Action<SkillDragPayload> OnDragBegin;

    /// <summary>
    /// 拖拽结束事件（无论是否成功 drop 都会触发）
    /// </summary>
    public event Action OnDragEnd;

    /// <summary>
    /// 外部控制是否允许拖拽。
    /// 技能树节点：仅 Unlocked 状态为 true
    /// 技能槽：仅装备了技能时为 true
    /// </summary>
    public bool canDrag = false;

    /// <summary>
    /// 当前拖拽数据。接收方（OnDrop）从这里读取，而不是直接 GetComponent 拿私有字段。
    /// 未在拖拽中时 payload 为默认值（skillType = None）。
    /// </summary>
    public SkillDragPayload Payload => payload;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvas = GetComponentInParent<Canvas>();

        // canDrag 默认为 false：初始禁用本组件，避免拖拽接口吞掉父物体 Button 的点击
        enabled = canDrag;
    }

    /// <summary>
    /// 初始化拖拽数据（由外部调用，如技能树节点 Init 或技能槽 Refresh 时）
    /// </summary>
    /// <param name="type">技能类型</param>
    /// <param name="skillIcon">技能图标 Image</param>
    public void Init(SkillType type, Image skillIcon)
    {
        payload = new SkillDragPayload
        {
            skillType = type,
            iconSprite = skillIcon != null ? skillIcon.sprite : null,
            originalColor = skillIcon != null ? skillIcon.color : Color.white
        };
        icon = skillIcon;
    }

    /// <summary>
    /// 设置是否允许拖拽。
    /// 关键：同时禁用/启用本组件。被禁用的 MonoBehaviour 不接收任何事件接口，
    /// 否则 IBeginDragHandler 会在鼠标轻微移动时触发，导致 Unity 判定为拖拽、取消点击，
    /// 父物体 Button（解锁技能）永远收不到 OnPointerClick。
    /// </summary>
    public void SetCanDrag(bool enabled)
    {
        canDrag = enabled;
        this.enabled = enabled;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 不允许拖拽则直接退出
        if (!canDrag)
        {
            Debug.Log("该技能不可拖拽:" + payload.skillType);
            return;
        }

        Debug.Log("开始拖拽技能:" + payload.skillType);

        // 禁止原对象接收射线，让 OnDrop 能被技能槽等接收方捕获
        canvasGroup.blocksRaycasts = false;

        // 拖拽开始时重新确认 sprite，确保是最新的（防止 Init 后图标被改过）
        if (icon != null && icon.sprite != null)
        {
            payload.iconSprite = icon.sprite;
        }

        // 记录原图标位置和大小到 payload，供回弹动画和接收方使用
        payload.originalPosition = transform.position;
        RectTransform source = GetComponent<RectTransform>();
        payload.originalSize = source.sizeDelta;

        // 原图标渐变为半透明，表示"被拿起"
        if (icon != null)
        {
            StopAnimCoroutine();
            currentAnimCoroutine = StartCoroutine(FadeIconAlpha(dragAlpha, fadeDuration));
        }

        // 创建跟随鼠标的拖拽图标（优先用预制体，没有则运行时动态创建）
        CreateDragIcon();

        // 播放弹出动画：scale 从 0 → popScale → 1，带弹性
        StopAnimCoroutine();
        currentAnimCoroutine = StartCoroutine(PopUpAnim());

        // 触发拖拽开始事件
        OnDragBegin?.Invoke(payload);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            // 保持偏移跟随鼠标
            dragIconRect.position = (Vector2)Input.mousePosition + dragOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 恢复原对象的射线接收
        canvasGroup.blocksRaycasts = true;

        // 判断是否落在了有效的 drop 目标上
        bool droppedOnValidTarget = eventData.pointerEnter != null
            && eventData.pointerEnter.GetComponentInParent<IDropHandler>() != null;

        if (droppedOnValidTarget)
        {
            // 落在有效目标上：快速淡出 + 销毁（OnDrop 会处理装备逻辑）
            StopAnimCoroutine();
            currentAnimCoroutine = StartCoroutine(QuickFadeOutAndDestroy());
        }
        else
        {
            // 拖到空白处：回弹动画 → 飞回原位 → 恢复原图标 → 销毁
            StopAnimCoroutine();
            currentAnimCoroutine = StartCoroutine(SnapBackAnim());
        }

        // 触发拖拽结束事件
        OnDragEnd?.Invoke();
    }

    #region ===== 拖拽图标创建 =====

    /// <summary>
    /// 创建拖拽图标：优先使用预制体，没有则运行时动态创建（向后兼容）
    /// 大小与原图标保持一致，通过 preserveAspect 保证比例正确
    /// </summary>
    private void CreateDragIcon()
    {
        if (dragIconPrefab != null)
        {
            // 使用预制体：可在预制体里配置初始样式、特效等
            dragIcon = Instantiate(dragIconPrefab, canvas.transform, false);
            dragIconRect = dragIcon.GetComponent<RectTransform>();
            // 优先找根节点的 Image，找不到再找子物体（避免误取到背景框等其他 Image）
            dragIconImage = dragIcon.GetComponent<Image>();
            if (dragIconImage == null)
            {
                dragIconImage = dragIcon.GetComponentInChildren<Image>();
            }

            // 设置图标 sprite（预制体可能是占位图，运行时替换）
            if (dragIconImage != null && payload.iconSprite != null)
            {
                dragIconImage.sprite = payload.iconSprite;
                dragIconImage.raycastTarget = false;
                // 保持原始宽高比，防止图标被拉伸变形
                dragIconImage.preserveAspect = true;
            }

            // 大小与原图标保持一致（不是 sprite 原生像素大小，而是 UI 上显示的大小）
            if (dragIconRect != null)
            {
                dragIconRect.sizeDelta = payload.originalSize;
            }
        }
        else
        {
            // 无预制体时运行时动态创建（兼容旧版）
            dragIcon = new GameObject("DragIcon");
            dragIcon.transform.SetParent(canvas.transform, false);
            dragIconImage = dragIcon.AddComponent<Image>();
            dragIconImage.sprite = payload.iconSprite;
            dragIconImage.raycastTarget = false;
            // 保持原始宽高比，防止图标被拉伸变形
            dragIconImage.preserveAspect = true;

            dragIconRect = dragIcon.GetComponent<RectTransform>();
            // 大小与原图标保持一致（不是 sprite 原生像素大小，而是 UI 上显示的大小）
            dragIconRect.sizeDelta = payload.originalSize;

            CanvasGroup group = dragIcon.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;
        }

        // 初始位置：鼠标 + 偏移；初始 scale：0（从 0 弹起的动画效果）
        dragIconRect.position = (Vector2)Input.mousePosition + dragOffset;
        dragIconRect.localScale = Vector3.zero;
    }

    #endregion

    #region ===== 动画协程 =====

    /// <summary>
    /// 弹出动画：scale 从 0 弹到 popScale，再回落到 1，带轻微弹性
    /// </summary>
    private IEnumerator PopUpAnim()
    {
        float timer = 0f;
        // 前半段：0 → popScale（快速放大）
        float firstHalf = popDuration * 0.6f;
        // 后半段：popScale → 1（回落到正常大小）
        float secondHalf = popDuration * 0.4f;

        while (timer < firstHalf)
        {
            timer += Time.deltaTime;
            float t = timer / firstHalf;
            // EaseOutCubic 曲线，先快后慢
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            dragIconRect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * popScale, eased);
            yield return null;
        }

        timer = 0f;
        while (timer < secondHalf)
        {
            timer += Time.deltaTime;
            float t = timer / secondHalf;
            float eased = 1f - Mathf.Pow(1f - t, 2f);
            dragIconRect.localScale = Vector3.Lerp(Vector3.one * popScale, Vector3.one, eased);
            yield return null;
        }

        dragIconRect.localScale = Vector3.one;
    }

    /// <summary>
    /// 回弹动画：拖拽图标飞回原位置，同时缩小，原图标同步恢复透明度
    /// </summary>
    private IEnumerator SnapBackAnim()
    {
        Vector3 startPos = dragIconRect.position;
        Vector3 startScale = dragIconRect.localScale;
        float timer = 0f;

        // 原图标透明度也同步恢复
        if (icon != null)
        {
            float startAlpha = icon.color.a;
            while (timer < snapBackDuration)
            {
                timer += Time.deltaTime;
                float t = timer / snapBackDuration;
                // EaseIn 曲线，先慢后快（像被吸回去）
                float eased = t * t;

                // 位置插值飞回原位
                dragIconRect.position = Vector3.Lerp(startPos, payload.originalPosition, eased);
                // 逐渐缩小到 0
                dragIconRect.localScale = Vector3.Lerp(startScale, Vector3.zero, eased);
                // 原图标透明度同步恢复
                Color c = icon.color;
                c.a = Mathf.Lerp(startAlpha, payload.originalColor.a, eased);
                icon.color = c;

                yield return null;
            }
            // 确保最终颜色正确
            icon.color = payload.originalColor;
        }
        else
        {
            // 没有原图标引用时只做位置和缩放动画
            while (timer < snapBackDuration)
            {
                timer += Time.deltaTime;
                float t = timer / snapBackDuration;
                float eased = t * t;

                dragIconRect.position = Vector3.Lerp(startPos, payload.originalPosition, eased);
                dragIconRect.localScale = Vector3.Lerp(startScale, Vector3.zero, eased);

                yield return null;
            }
        }

        Destroy(dragIcon);
    }

    /// <summary>
    /// 快速淡出并销毁（成功装备时用）
    /// </summary>
    private IEnumerator QuickFadeOutAndDestroy()
    {
        float timer = 0f;
        float duration = 0.1f;
        Color startColor = dragIconImage.color;

        // 原图标也同步恢复透明度
        if (icon != null)
        {
            float startAlpha = icon.color.a;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;

                // 拖拽图标淡出
                Color c = dragIconImage.color;
                c.a = Mathf.Lerp(startColor.a, 0f, t);
                dragIconImage.color = c;

                // 原图标淡入
                Color ic = icon.color;
                ic.a = Mathf.Lerp(startAlpha, payload.originalColor.a, t);
                icon.color = ic;

                yield return null;
            }
            icon.color = payload.originalColor;
        }
        else
        {
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;

                Color c = dragIconImage.color;
                c.a = Mathf.Lerp(startColor.a, 0f, t);
                dragIconImage.color = c;

                yield return null;
            }
        }

        Destroy(dragIcon);
    }

    /// <summary>
    /// 原图标透明度渐变
    /// </summary>
    private IEnumerator FadeIconAlpha(float targetAlpha, float duration)
    {
        float timer = 0f;
        float startAlpha = icon.color.a;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            Color c = icon.color;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            icon.color = c;

            yield return null;
        }

        Color final = icon.color;
        final.a = targetAlpha;
        icon.color = final;
    }

    /// <summary>
    /// 停止当前正在运行的动画协程，避免动画冲突
    /// </summary>
    private void StopAnimCoroutine()
    {
        if (currentAnimCoroutine != null)
        {
            StopCoroutine(currentAnimCoroutine);
            currentAnimCoroutine = null;
        }
    }

    #endregion
}
