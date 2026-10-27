using UnityEngine;
using System;
using MiniRPG.Core;

/// <summary>
/// 玩家金币管理：记录金币数量，变更时触发事件通知 UI 刷新。
/// 金币获取：打碎金矿后拾取掉落金币（GoldCoin.Pickup 调用 AddGold）
/// 金币消耗：商店购买（TrySpend）
/// 持久化到 GameManager.SaveData.gold。
/// </summary>
public class PlayerGold : MonoBehaviour
{
    [Header("金币")]
    [Tooltip("初始金币数（无存档时使用）")]
    [SerializeField]
    private int initGold = 0;

    private int currentGold;

    // 金币变更事件，UI 订阅后刷新显示
    public event Action OnGoldChanged;

    private void Awake()
    {
        // 从存档读取，无存档则用 Inspector 初始值
        currentGold = GameManager.Instance?.SaveData != null
            ? GameManager.Instance.SaveData.gold
            : initGold;
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
        Persist();
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
        Persist();
        OnGoldChanged?.Invoke();
        Debug.Log("花费金币:" + amount + " 剩余:" + currentGold);
        return true;
    }

    /// <summary>把当前金币写入存档</summary>
    private void Persist()
    {
        if (GameManager.Instance?.SaveData != null)
        {
            GameManager.Instance.SaveData.gold = currentGold;
            GameManager.Instance.Save();
        }
    }
}
