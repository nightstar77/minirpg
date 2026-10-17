using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 技能拖拽数据包：在拖拽开始时创建，通过 SkillDragHandler.Payload 对外暴露。
/// 接收方（如 SkillSlotUI.OnDrop）从拖拽源读取这个结构体获取技能信息，
/// 而不是直接 GetComponent<SkillDragHandler>().SkillType，避免拖拽源和接收方直接耦合。
/// 未来扩展拖拽内容（如技能描述、等级等）只需加字段，不用改调用方代码。
/// </summary>
public struct SkillDragPayload
{
    /// <summary>技能类型（核心标识）</summary>
    public SkillType skillType;

    /// <summary>技能图标 Sprite（用于显示拖拽中的图标）</summary>
    public Sprite iconSprite;

    /// <summary>技能名称（用于 Debug 或 tooltip）</summary>
    public string skillName;

    /// <summary>原始位置（屏幕坐标），用于回弹动画</summary>
    public Vector3 originalPosition;

    /// <summary>原始大小，用于回弹时 scale 归零</summary>
    public Vector2 originalSize;

    /// <summary>原始颜色，用于恢复原图标颜色</summary>
    public Color originalColor;
}

/// <summary>
/// 拖拽源接口：任何可以被拖拽到技能槽的对象都实现它（技能树节点、技能槽等）。
/// 接收方（SkillSlotUI.OnDrop）通过它读取拖拽数据，不关心拖拽源的具体类型，
/// 新增拖拽源时不用改接收方代码。
/// </summary>
public interface ISkillDragSource
{
    SkillDragPayload Payload { get; }
}
