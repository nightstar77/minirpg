using System;
using System.Collections.Generic;

namespace MiniRPG.Core
{
    /// <summary>
    /// 全局存档数据：所有需要持久化的玩家进度与设置统一存于此。
    /// 序列化为 JSON 写入 Application.persistentDataPath/save.json。
    /// 带 version 字段，便于后续字段增删时做迁移。
    /// </summary>
    [Serializable]
    public class SaveData
    {
        /// <summary>存档版本号，字段结构变化时递增</summary>
        public int version = 1;

        // ===== 玩家进度 =====
        public int level = 1;
        public int exp = 0;
        public int gold = 0;
        public int skillPoints = 0;

        /// <summary>已解锁技能（SkillType 的 int 值列表）</summary>
        public List<int> unlockedSkills = new List<int>();

        /// <summary>强化类型（UpgradeType 的 int 值），与 upgradeLevels 一一对应</summary>
        public List<int> upgradeTypes = new List<int>();
        public List<int> upgradeLevels = new List<int>();

        // ===== 设置 =====
        public float bgmVolume = 0.5f;
        public float sfxVolume = 1f;

        /// <summary>按键绑定：动作（GameInputs.Action 的 int 值），与 keyCodes 一一对应</summary>
        public List<int> keyActions = new List<int>();
        public List<int> keyCodes = new List<int>();
    }
}
