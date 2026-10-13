using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 商店中单个强化项的 UI：显示名称、等级、价格，点击按钮购买。
/// 由 UpgradeShopUI 在打开商店时调用 Init 绑定数据与管理器。
/// </summary>
public class UpgradeItemUI : MonoBehaviour
{
    [Header("UI 组件（拖拽赋值）")]
    [Tooltip("强化名称文本")]
    [SerializeField]
    private TMP_Text nameText;

    [Tooltip("等级文本，如「等级 2/5」")]
    [SerializeField]
    private TMP_Text levelText;

    [Tooltip("价格文本，如「100 金币」")]
    [SerializeField]
    private TMP_Text costText;

    [Tooltip("购买按钮")]
    [SerializeField]
    private Button buyButton;

    // 绑定的数据与管理器（由 UpgradeShopUI.Init 注入）
    private UpgradeData data;
    private PlayerUpgrades manager;

    private void Start()
    {
        // 按钮点击事件：尝试购买并刷新
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyClicked);
        }
    }

    /// <summary>
    /// 绑定数据并刷新显示
    /// </summary>
    public void Init(UpgradeData upgradeData, PlayerUpgrades upgradesManager)
    {
        data = upgradeData;
        manager = upgradesManager;
        Refresh();
    }

    /// <summary>
    /// 刷新显示（等级、价格、按钮可点击状态）
    /// </summary>
    public void Refresh()
    {
        if (data == null || manager == null) return;

        if (nameText != null)
        {
            nameText.text = data.upgradeName;
        }

        int level = manager.GetLevel(data);
        if (levelText != null)
        {
            levelText.text = $"等级 {level}/{data.maxLevel}";
        }

        bool isMax = manager.IsMaxLevel(data);
        if (costText != null)
        {
            costText.text = isMax ? "已满级" : $"{manager.GetNextCost(data)} 金币";
        }

        if (buyButton != null)
        {
            // 已满级或金币不足时按钮不可点击
            buyButton.interactable = manager.CanBuy(data);
        }
    }

    private void OnBuyClicked()
    {
        if (manager != null && data != null)
        {
            manager.TryBuy(data);
            // 购买后刷新所有 UI（由商店统一刷新，这里也刷一下自己）
            Refresh();
        }
    }
}
