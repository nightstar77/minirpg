using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家经验条 UI
/// 设计模式参考 SkillPointUI：订阅事件刷新，不每帧轮询
/// - 订阅 PlayerExperience.OnExpChanged → 刷新经验进度条
/// - 订阅 PlayerExperience.OnLevelChanged → 刷新等级文本
///
/// 使用方式：
/// 1. Canvas 下放一个背景 Image，子物体放一个填充 Image（Image Type = Filled）
/// 2. 挂此脚本，拖入 playerExperience、expBar、levelText
/// 3. 经验数值文本可选（如 "80/100"），不拖就不显示
/// </summary>
public class PlayerExpUI : MonoBehaviour
{
    [Header("数据来源（拖拽赋值）")]
    [Tooltip("玩家经验系统组件")]
    [SerializeField]
    private PlayerExperience playerExperience;

    [Header("UI 组件（拖拽赋值）")]
    [Tooltip("经验条填充图（Image Type 必须设为 Filled，Fill Method = Horizontal）")]
    [SerializeField]
    private Image expBar;

    [Tooltip("等级文本，如 Lv.1")]
    [SerializeField]
    private TMP_Text levelText;

    [Tooltip("经验数值文本，如 80/100（可选，不需要就留空）")]
    [SerializeField]
    private TMP_Text expText;

    private void Start()
    {
        if (playerExperience == null) return;

        // 订阅经验与等级变化事件
        playerExperience.OnExpChanged += RefreshExpBar;
        playerExperience.OnLevelChanged += RefreshLevel;

        // 初始刷新一次
        RefreshExpBar(playerExperience.CurrentExp, playerExperience.ExpToNextLevel);
        RefreshLevel(playerExperience.CurrentLevel);
    }

    private void OnDestroy()
    {
        // 取消订阅，避免空引用
        if (playerExperience == null) return;
        playerExperience.OnExpChanged -= RefreshExpBar;
        playerExperience.OnLevelChanged -= RefreshLevel;
    }

    /// <summary>
    /// 刷新经验条（由 OnExpChanged 事件触发）
    /// </summary>
    private void RefreshExpBar(int currentExp, int expToNext)
    {
        // 更新填充比例
        if (expBar != null && expToNext > 0)
        {
            expBar.fillAmount = (float)currentExp / expToNext;
        }

        // 更新数值文本（如果配了）
        if (expText != null)
        {
            expText.text = currentExp + "/" + expToNext;
        }
    }

    /// <summary>
    /// 刷新等级文本（由 OnLevelChanged 事件触发）
    /// </summary>
    private void RefreshLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = "Lv." + level;
        }
    }
}
