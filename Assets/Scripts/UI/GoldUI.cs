using TMPro;
using UnityEngine;

/// <summary>
/// 金币数量 UI：订阅 PlayerGold.OnGoldChanged 事件刷新显示，不每帧轮询
/// 设计模式参考 PlayerExpUI
///
/// 使用方式：
/// 1. Canvas 下放金币图标 Image（拖你的金币图标）+ 金币数量 TMP 文本
/// 2. 挂此脚本，拖入 playerGold、goldText
/// </summary>
public class GoldUI : MonoBehaviour
{
    [Header("数据来源（拖拽赋值）")]
    [Tooltip("玩家金币组件（挂在 Player 物体上）")]
    [SerializeField]
    private PlayerGold playerGold;

    [Header("UI 组件（拖拽赋值）")]
    [Tooltip("金币数量文本")]
    [SerializeField]
    private TMP_Text goldText;

    private void Start()
    {
        if (playerGold == null) return;

        // 订阅金币变更事件
        playerGold.OnGoldChanged += RefreshGold;
        RefreshGold();   // 初始刷新一次
    }

    private void OnDestroy()
    {
        // 取消订阅，避免空引用
        if (playerGold == null) return;
        playerGold.OnGoldChanged -= RefreshGold;
    }

    /// <summary>
    /// 刷新金币显示（由 OnGoldChanged 事件触发）
    /// </summary>
    private void RefreshGold()
    {
        if (goldText != null)
        {
            goldText.text = playerGold.CurrentGold.ToString();
        }
    }
}
