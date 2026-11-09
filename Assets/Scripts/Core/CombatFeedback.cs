using UnityEngine;

namespace MiniRPG
{
    /// <summary>
    /// 战斗反馈统一入口：命中时同时触发命中停顿、屏幕震动、伤害飘字。
    /// 玩家武器（WeaponHitBox）与敌人弹道（AttackPrefab）命中后调用，
    /// 让打击表现的调用点保持集中，方便后续统一调节手感参数。
    /// </summary>
    public static class CombatFeedback
    {
        /// <summary>
        /// 命中反馈
        /// </summary>
        /// <param name="worldPos">命中位置</param>
        /// <param name="damage">伤害数值</param>
        /// <param name="isPlayerHit">true 表示玩家被打（红色飘字 + 更强震动）</param>
        public static void OnHit(Vector3 worldPos, float damage, bool isPlayerHit)
        {
            // 暂停时不播放任何打击反馈
            if (GameManager.Instance.IsPaused) { return; }

            // 命中停顿（默认 0.05 秒，压低到 0.05 倍速）
            GameManager.Instance.PlayHitStop(0.05f, 0.05f);

            // 屏幕震动：玩家挨打更明显
            CameraShake.ShakeCamera(0.12f, isPlayerHit ? 0.15f : 0.08f);

            // 伤害飘字：玩家受伤红色，敌人受伤白色
            Color color = isPlayerHit ? new Color(1f, 0.3f, 0.3f) : Color.white;
            DamageNumbers.Show(worldPos, damage, color);
        }

        /// <summary>
        /// 格挡反馈：玩家防御成功时调用，不显示伤害数字，改为格挡提示 + 轻微震屏。
        /// </summary>
        /// <param name="worldPos">玩家被击中的位置</param>
        public static void OnBlock(Vector3 worldPos)
        {
            if (GameManager.Instance.IsPaused) { return; }

            CameraShake.ShakeCamera(0.08f, 0.05f);
            DamageNumbers.ShowText(worldPos, "格挡", new Color(0.75f, 0.85f, 1f));
        }
    }
}
