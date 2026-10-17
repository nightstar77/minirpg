using UnityEngine;
using System;

/// <summary>
/// 玩家金币管理：记录金币数量，变更时触发事件通知 UI 刷新。
/// 金币获取：打碎金矿后拾取掉落金币（GoldCoin.Pickup 调用 AddGold）
/// 金币消耗：预留给商店购买等功能（TrySpend）
/// 设计模式参考 PlayerSkillPoint
/// </summary>
public class PlayerGold : MonoBehaviour
{
    [Header("金币")]
    [Tooltip("初始金币数")]
    [SerializeField]
    private int initGold = 0;

    private int currentGold;

    // 金币变更事件，UI 订阅后刷新显示
    public event Action OnGoldChanged;

    private void Awake()
    {
        currentGold = initGold;
    }

    /// <summary>
    /// 当前金币数
    /// </summary>
    public int CurrentGold
    {
        get { return currentGold; }
    }

    /// <summary>
    /// 获得金币（拾取掉落金币、任务奖励等）
    /// </summary>
    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }
        currentGold += amount;
        OnGoldChanged?.Invoke();
        Debug.Log("获得金币:" + amount + " 当前:" + currentGold);
    }

    /// <summary>
    /// 尝试花费金币，金币足够才扣减并返回 true
    /// </summary>
    public bool TrySpend(int amount)
    {
        if (currentGold < amount)
        {
            Debug.Log("金币不足");
            return false;
        }
        currentGold -= amount;
        OnGoldChanged?.Invoke();
        Debug.Log("花费金币:" + amount + " 剩余:" + currentGold);
        return true;
    }
}
