using System;
using UnityEngine;
using MiniRPG.Core;

/// <summary>
/// 玩家经验与等级系统
///
/// 职责：
/// - 管理当前经验值、当前等级
/// - 击杀怪物后调用 GainExp() 获取经验
/// - 经验满后自动升级（支持一次经验连升多级）
/// - 每升 1 级自动获得 1 个技能点（调用 PlayerSkillPoint.AddPoint）
/// - 等级/经验持久化到 GameManager.SaveData
///
/// 升级经验公式（支持无限等级，不写死数组）：
///     升到 (当前等级+1) 所需经验 = baseExpToNext * 当前等级
/// </summary>
public class PlayerExperience : MonoBehaviour
{
    [Header("等级设置")]
    [Tooltip("初始等级（一般为 1），无存档时使用")]
    public int startLevel = 1;

    [Header("经验曲线设置")]
    [Tooltip("升级所需经验的基础值：下级所需经验 = 此值 * 当前等级")]
    public int baseExpToNext = 100;

    [Header("组件引用（拖拽赋值）")]
    [Tooltip("玩家技能点管理器，升级时自动加 1 点")]
    public PlayerSkillPoint playerSkillPoint;

    // ===== 运行时数据 =====
    private int currentLevel;
    private int currentExp;
    // 当前等级升到下一级所需的总经验（每次升级时按公式重算）
    private int expToNextLevel;

    // ===== 事件（UI 订阅） =====
    /// <summary>经验变化事件（参数：当前经验、下级所需经验）</summary>
    public event Action<int, int> OnExpChanged;

    /// <summary>等级变化事件（参数：新等级）</summary>
    public event Action<int> OnLevelChanged;

    /// <summary>当前等级（只读）</summary>
    public int CurrentLevel => currentLevel;

    /// <summary>当前经验（只读）</summary>
    public int CurrentExp => currentExp;

    /// <summary>升到下一级所需经验（只读）</summary>
    public int ExpToNextLevel => expToNextLevel;

    private void Awake()
    {
        // 从存档读取等级/经验，无存档则用初始值
        var save = GameManager.Instance?.SaveData;
        currentLevel = save != null ? save.level : startLevel;
        currentExp = save != null ? save.exp : 0;
        RecalculateExpToNext();
    }

    private void Start()
    {
        // 通知 UI 初始显示
        OnExpChanged?.Invoke(currentExp, expToNextLevel);
        OnLevelChanged?.Invoke(currentLevel);
    }

    /// <summary>
    /// 获取经验（由怪物死亡时调用）
    /// 支持一次经验连升多级（比如打大怪直接升两级）
    /// </summary>
    public void GainExp(int amount)
    {
        if (amount <= 0) return;

        currentExp += amount;
        Debug.Log("获得经验:" + amount + " 当前经验:" + currentExp);

        // 经验可能连升多级，用 while 循环
        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

        Persist();
        // 通知 UI 刷新经验条
        OnExpChanged?.Invoke(currentExp, expToNextLevel);
    }

    /// <summary>
    /// 升级：等级 +1，扣除升级所需经验，发放 1 个技能点
    /// </summary>
    private void LevelUp()
    {
        // 扣除本级所需经验，溢出经验带到下一级
        currentExp -= expToNextLevel;
        currentLevel++;

        // 重新计算下一级所需经验
        RecalculateExpToNext();

        // 每升一级给 1 个技能点
        if (playerSkillPoint != null)
        {
            playerSkillPoint.AddPoint(1);
        }

        Debug.Log("升级！当前等级:" + currentLevel);

        // 播放升级音效
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLevelUp();
        }

        // 通知 UI 刷新等级显示
        OnLevelChanged?.Invoke(currentLevel);
    }

    /// <summary>
    /// 按公式重算升级所需经验
    /// 公式：baseExpToNext * 当前等级
    /// </summary>
    private void RecalculateExpToNext()
    {
        expToNextLevel = baseExpToNext * currentLevel;
    }

    /// <summary>把当前等级/经验写入存档</summary>
    private void Persist()
    {
        if (GameManager.Instance?.SaveData != null)
        {
            GameManager.Instance.SaveData.level = currentLevel;
            GameManager.Instance.SaveData.exp = currentExp;
            GameManager.Instance.Save();
        }
    }
}
