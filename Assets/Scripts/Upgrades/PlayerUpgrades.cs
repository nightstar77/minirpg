using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家强化管理器：记录所有强化项的等级，购买时扣除金币并把效果应用到玩家属性。
/// 挂在 Player 物体上。
///
/// 强化效果应用目标：
/// - MaxHealth   → PlayerHealth.IncreaseMaxHP
/// - AttackDamage → WeaponHitBox.damage
/// - AttackSpeed  → PlayerAnimation.SetAnimationSpeed
///
/// 设计模式参考 PlayerSkillPoint（事件通知 UI 刷新）。
/// </summary>
public class PlayerUpgrades : MonoBehaviour
{
    [Header("可购买的强化项列表（拖拽 UpgradeData 资产）")]
    [Tooltip("强化商店中可购买的所有强化项")]
    [SerializeField]
    private UpgradeData[] upgrades;

    #region ===== Component =====
    private PlayerHealth playerHealth;
    private WeaponHitBox weapon;
    private PlayerAnimation playerAnimation;
    private PlayerGold playerGold;
    #endregion

    #region ===== Runtime =====
    // 每项强化的当前等级
    private Dictionary<UpgradeData, int> upgradeLevels = new Dictionary<UpgradeData, int>();
    // 攻击速度累积增量（每次强化累加，最终 animator.speed = 1 + 此值）
    private float attackSpeedBonus = 0f;

    // 强化变更事件，UI 订阅后刷新显示
    public event Action OnUpgradeChanged;
    #endregion

    #region ===== Unity =====
    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        weapon = GetComponentInChildren<WeaponHitBox>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerGold = GetComponent<PlayerGold>();

        // 初始化所有强化项等级为 0
        if (upgrades != null)
        {
            foreach (UpgradeData data in upgrades)
            {
                if (data != null && !upgradeLevels.ContainsKey(data))
                {
                    upgradeLevels.Add(data, 0);
                }
            }
        }
    }
    #endregion

    #region ===== Public API =====

    /// <summary>
    /// 所有可购买的强化项列表（供商店 UI 读取）
    /// </summary>
    public UpgradeData[] AvailableUpgrades => upgrades;

    /// <summary>
    /// 获取指定强化项的当前等级
    /// </summary>
    public int GetLevel(UpgradeData data)
    {
        if (data == null || !upgradeLevels.ContainsKey(data)) return 0;
        return upgradeLevels[data];
    }

    /// <summary>
    /// 获取指定强化项下一级的价格
    /// </summary>
    public int GetNextCost(UpgradeData data)
    {
        return data.GetCostForLevel(GetLevel(data));
    }

    /// <summary>
    /// 是否已达最大等级
    /// </summary>
    public bool IsMaxLevel(UpgradeData data)
    {
        return GetLevel(data) >= data.maxLevel;
    }

    /// <summary>
    /// 是否有足够金币且未满级
    /// </summary>
    public bool CanBuy(UpgradeData data)
    {
        if (data == null || IsMaxLevel(data)) return false;
        return playerGold != null && playerGold.CurrentGold >= GetNextCost(data);
    }

    /// <summary>
    /// 尝试购买强化。成功则扣金币、升级、应用效果、通知 UI。
    /// </summary>
    public bool TryBuy(UpgradeData data)
    {
        if (!CanBuy(data))
        {
            Debug.Log("无法购买强化：" + (IsMaxLevel(data) ? "已满级" : "金币不足"));
            return false;
        }

        int cost = GetNextCost(data);
        if (!playerGold.TrySpend(cost))
        {
            return false;
        }

        upgradeLevels[data]++;
        ApplyUpgrade(data);

        Debug.Log($"购买强化 {data.upgradeName} 等级 {upgradeLevels[data]}/{data.maxLevel}，花费 {cost}");
        OnUpgradeChanged?.Invoke();
        return true;
    }

    #endregion

    #region ===== Effect Apply =====
    /// <summary>
    /// 把强化效果应用到玩家对应属性
    /// </summary>
    private void ApplyUpgrade(UpgradeData data)
    {
        switch (data.type)
        {
            case UpgradeType.MaxHealth:
                if (playerHealth != null)
                {
                    playerHealth.IncreaseMaxHP(data.amountPerLevel);
                }
                break;

            case UpgradeType.AttackDamage:
                if (weapon != null)
                {
                    weapon.damage += data.amountPerLevel;
                    Debug.Log("攻击力提升 +" + data.amountPerLevel + " 当前:" + weapon.damage);
                }
                break;

            case UpgradeType.AttackSpeed:
                attackSpeedBonus += data.amountPerLevel;
                if (playerAnimation != null)
                {
                    playerAnimation.SetAnimationSpeed(1f + attackSpeedBonus);
                }
                Debug.Log("攻击速度提升 +" + data.amountPerLevel + " 当前倍率:" + (1f + attackSpeedBonus));
                break;
        }
    }
    #endregion
}
