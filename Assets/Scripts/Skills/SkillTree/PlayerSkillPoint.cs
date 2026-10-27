using UnityEngine;
using System;
using MiniRPG.Core;

/// <summary>
/// 玩家技能点管理：消耗与获取技能点，变更时触发事件通知 UI 刷新。
/// 技能点获取方式：玩家每升 1 级由 PlayerExperience 自动发放 1 点。
/// 持久化到 GameManager.SaveData.skillPoints。
/// </summary>
public class PlayerSkillPoint : MonoBehaviour
{
    [Header("技能点")]
    [Tooltip("初始技能点数（默认 0，技能点通过升级获得；调试时可临时改大）")]
    [SerializeField]
    private int skillPoints = 0;
    private int currentPoint;

    // 技能点变更事件，UI 订阅后刷新显示
    public event Action OnSkillPointChanged;

    private void Awake()
    {
        // 从存档读取，无存档则用 Inspector 初始值
        currentPoint = GameManager.Instance?.SaveData != null
            ? GameManager.Instance.SaveData.skillPoints
            : skillPoints;
    }

    public int CurrentPoint
    {
        get { return currentPoint; }
    }

    public bool CanSpend(int cost)
    {
        return currentPoint >= cost;
    }

    public bool SpendPoint(int amount)
    {
        if (currentPoint < amount)
        {
            Debug.Log("技能点不足");
            return false;
        }
        currentPoint -= amount;
        Persist();
        OnSkillPointChanged?.Invoke();

        Debug.Log("消耗技能点:" + amount + " 剩余:" + currentPoint);
        return true;
    }

    public void AddPoint(int amount)
    {
        currentPoint += amount;
        Persist();
        OnSkillPointChanged?.Invoke();

        Debug.Log("获得技能点:" + amount);
    }

    /// <summary>把当前技能点写入存档</summary>
    private void Persist()
    {
        if (GameManager.Instance?.SaveData != null)
        {
            GameManager.Instance.SaveData.skillPoints = currentPoint;
            GameManager.Instance.Save();
        }
    }
}