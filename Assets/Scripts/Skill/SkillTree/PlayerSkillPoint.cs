using UnityEngine;
using System;

/// <summary>
/// 玩家技能点管理：消耗与获取技能点，变更时触发事件通知 UI 刷新
/// </summary>
public class PlayerSkillPoint : MonoBehaviour
{
    [Header("技能点")]
    [SerializeField]
    private int skillPoints = 5;
    private int currentPoint;

    // 技能点变更事件，UI 订阅后刷新显示
    public event Action OnSkillPointChanged;

    private void Awake()
    {
        currentPoint = skillPoints;
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
        OnSkillPointChanged?.Invoke();

        Debug.Log("消耗技能点:" + amount + " 剩余:" + currentPoint);
        return true;
    }

    public void AddPoint(int amount)
    {
        currentPoint += amount;
        OnSkillPointChanged?.Invoke();

        Debug.Log("获得技能点:" + amount);
    }
}