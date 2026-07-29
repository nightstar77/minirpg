using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private SkillType skillType;
    private CanvasGroup canvasGroup;
    private GameObject dragIcon;
    private SkillNodeUI nodeUI;
    private Image icon;

    public SkillType SkillType
    {
        get { return skillType; }
    }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvas = GetComponentInParent<Canvas>();
        nodeUI = GetComponent<SkillNodeUI>();
    }

    public void Init(SkillNodeData data, Image skillIcon)
    {
        skillType = data.skill.skillType;
        icon = skillIcon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("拖拽对象:" + gameObject.name);
        if (nodeUI != null)
        {
            if (nodeUI.CurrentState != SkillNodeState.Unlocked)
            { return; }
        }
        Debug.Log("开始拖拽技能:" + SkillType);
        //禁止原对象接收射线
        canvasGroup.blocksRaycasts = false;
        //创建拖拽图片
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform, false);
        Image image = dragIcon.AddComponent<Image>();
        image.sprite = icon.sprite;
        image.raycastTarget = false;
        RectTransform rect = dragIcon.GetComponent<RectTransform>();
        RectTransform source = GetComponent<RectTransform>();
        rect.sizeDelta = source.sizeDelta;
        CanvasGroup group = dragIcon.AddComponent<CanvasGroup>();
        rect.position = Input.mousePosition;
        group.blocksRaycasts = false;
        //dragIcon.transform.position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        { dragIcon.transform.position = Input.mousePosition; }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        if (dragIcon != null)
        { Destroy(dragIcon); }
    }

}