using NPC;
using UnityEngine;

/// <summary>
/// 强化商店 NPC：继承自 NpcBase，对话结束后自动打开强化商店面板。
///
/// 使用方式：
/// 1. 把此脚本挂到第二个 NPC 物体上（替代 NpcBase）
/// 2. 配置和普通 NPC 一样的组件：spriteRenderer、dialogueData、interactPrompt
/// 3. 子物体放交互检测区（NpcInteractCollider）
/// 4. 场景里要有 UpgradeShopUI
/// </summary>
public class UpgradeShopNpc : NpcBase
{
    /// <summary>
    /// 对话结束回调：打开强化商店面板
    /// </summary>
    protected override void OnDialogueEnded()
    {
        if (UpgradeShopUI.Instance != null)
        {
            UpgradeShopUI.Instance.Open();
        }
        else
        {
            Debug.LogWarning("场景中没有 UpgradeShopUI，无法打开强化商店");
        }
    }
}
