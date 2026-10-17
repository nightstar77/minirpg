using UnityEngine;

/// <summary>
/// 强化项配置数据（ScriptableObject）
/// 创建方式：Project 窗口右键 → Create → Upgrade → Upgrade Data
///
/// 每项强化通过此资产配置：名称、描述、效果类型、每级增量、价格公式、最大等级。
/// 价格公式：下一级价格 = round(baseCost * costMultiplier^当前等级)
/// </summary>
[CreateAssetMenu(menuName = "Upgrade/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("强化名称，如「生命强化」")]
    public string upgradeName;

    [Tooltip("强化描述，如「每级最大生命值 +20」")]
    [TextArea]
    public string description;

    [Tooltip("强化图标（可选）")]
    public Sprite icon;

    [Header("效果")]
    [Tooltip("作用属性：最大生命 / 攻击力 / 攻击速度")]
    public UpgradeType type;

    [Tooltip("每级增加的数值（攻击速度用倍率增量，如 0.1 表示每级 +10%）")]
    public float amountPerLevel = 20f;

    [Header("价格与等级")]
    [Tooltip("第 1 级的价格")]
    public int baseCost = 50;

    [Tooltip("每升一级价格的倍率（1.5 = 每级涨 50%）")]
    public float costMultiplier = 1.5f;

    [Tooltip("最大等级")]
    public int maxLevel = 5;

    /// <summary>
    /// 计算指定等级的下一级价格
    /// </summary>
    public int GetCostForLevel(int currentLevel)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentLevel));
    }
}
