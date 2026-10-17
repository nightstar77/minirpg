using NPC;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 强化商店面板 UI（全局单例）
/// 玩家与强化商店 NPC 对话结束后打开，显示可购买的属性强化项。
///
/// 使用方式：
/// 1. Canvas 下建一个商店面板（背景 + 标题 + 若干个 UpgradeItemUI + 关闭按钮）
/// 2. 挂此脚本，拖入 itemSlots、closeButton、playerUpgrades、interactInput
/// 3. itemSlots 数量需与 PlayerUpgrades 中配置的强化项数量一致
/// </summary>
public class UpgradeShopUI : MonoBehaviour
{
    // 全局单例
    public static UpgradeShopUI Instance { get; private set; }

    [Header("引用（拖拽赋值）")]
    [Tooltip("玩家强化管理器（挂在 Player 上）")]
    [SerializeField]
    private PlayerUpgrades playerUpgrades;

    [Tooltip("商店里的强化项 UI 槽位，按顺序对应 PlayerUpgrades 的强化项列表")]
    [SerializeField]
    private UpgradeItemUI[] itemSlots;

    [Tooltip("关闭商店的按钮")]
    [SerializeField]
    private Button closeButton;

    [Tooltip("玩家交互输入脚本（打开商店时禁用，避免按 E 重新触发对话）")]
    [SerializeField]
    private InteractInput interactInput;

    private void Awake()
    {
        // 单例注册
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }

        // 初始隐藏
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #region ===== Open / Close =====

    /// <summary>
    /// 打开商店面板：绑定每个槽位的数据并刷新
    /// </summary>
    public void Open()
    {
        if (playerUpgrades == null)
        {
            Debug.LogError("UpgradeShopUI 没有配置 PlayerUpgrades");
            return;
        }

        gameObject.SetActive(true);

        // 禁用玩家交互输入，避免按 E 重新开始对话
        if (interactInput != null)
        {
            interactInput.enabled = false;
        }

        BindAndRefreshAll();
    }

    /// <summary>
    /// 关闭商店面板
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);

        // 恢复玩家交互输入
        if (interactInput != null)
        {
            interactInput.enabled = true;
        }
    }

    /// <summary>
    /// 刷新所有强化项显示（购买后金币变化时由 UI 调用）
    /// </summary>
    public void RefreshAll()
    {
        if (itemSlots == null) return;
        foreach (UpgradeItemUI item in itemSlots)
        {
            if (item != null) item.Refresh();
        }
    }

    #endregion

    #region ===== Internal =====

    /// <summary>
    /// 把 PlayerUpgrades 的强化项列表按顺序绑定到每个槽位并刷新
    /// </summary>
    private void BindAndRefreshAll()
    {
        if (itemSlots == null || playerUpgrades == null) return;

        // 订阅强化变更事件，确保购买后所有项同步刷新
        playerUpgrades.OnUpgradeChanged -= RefreshAll;
        playerUpgrades.OnUpgradeChanged += RefreshAll;

        UpgradeData[] allUpgrades = playerUpgrades.AvailableUpgrades;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == null) continue;

            if (i < allUpgrades.Length && allUpgrades[i] != null)
            {
                itemSlots[i].gameObject.SetActive(true);
                itemSlots[i].Init(allUpgrades[i], playerUpgrades);
            }
            else
            {
                // 没有对应强化项的槽位隐藏
                itemSlots[i].gameObject.SetActive(false);
            }
        }
    }

    #endregion
}
