using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局按键绑定管理器（静态类）：
/// 所有输入脚本（PlayerAttack/PlayerDefense/SkillInput）统一从这里读键，
/// 支持运行时改键并持久化到 PlayerPrefs。
///
/// 改键期间（IsCapturing=true）各输入脚本暂停读键，避免改键时误触发技能。
/// </summary>
public static class GameInputs
{
    #region ===== 动作定义 =====

    public enum Action
    {
        Attack,       // 攻击
        Defend,       // 防御
        Skill0,       // 技能槽 0
        Skill1,       // 技能槽 1
        Skill2,       // 技能槽 2
        Skill3,       // 技能槽 3
    }

    /// <summary>
    /// 技能槽对应的动作列表，索引即槽位索引
    /// </summary>
    public static readonly Action[] SkillActions =
    {
        Action.Skill0, Action.Skill1, Action.Skill2, Action.Skill3,
    };

    #endregion

    #region ===== 绑定数据 =====

    // 默认按键
    private static readonly Dictionary<Action, KeyCode> defaults = new Dictionary<Action, KeyCode>
    {
        { Action.Attack, KeyCode.J },
        { Action.Defend, KeyCode.K },
        { Action.Skill0, KeyCode.Q },
        { Action.Skill1, KeyCode.E },
        { Action.Skill2, KeyCode.R },
        { Action.Skill3, KeyCode.F },
    };

    private static Dictionary<Action, KeyCode> bindings;
    // v2：移除 CancelAttack 后枚举值整体前移，旧存档的动作索引会错位，换 key 废弃旧数据
    private const string SaveKey = "GameKeyBindings_v2";

    /// <summary>
    /// 绑定变更事件（改键后 UI 刷新用）。
    /// 注意用 System.Action 全名，因为本类定义了 enum Action 会遮蔽 System.Action
    /// </summary>
    public static event System.Action OnBindingsChanged;

    /// <summary>
    /// 是否正在等待玩家按键改键，此时所有游戏输入暂停
    /// </summary>
    public static bool IsCapturing { get; private set; }

    #endregion

    #region ===== 读取 =====

    public static KeyCode GetKey(Action action)
    {
        EnsureLoaded();
        return bindings[action];
    }

    /// <summary>
    /// 获取技能槽 N 的按键
    /// </summary>
    public static KeyCode GetSkillKey(int slotIndex)
    {
        EnsureLoaded();
        return bindings[SkillActions[slotIndex]];
    }

    /// <summary>
    /// 按键的显示名（None 显示"无"）
    /// </summary>
    public static string GetKeyName(Action action)
    {
        KeyCode code = GetKey(action);
        return code == KeyCode.None ? "无" : code.ToString();
    }

    #endregion

    #region ===== 改键 =====

    /// <summary>
    /// 设置某动作的按键。如果该键已被其他动作占用，旧动作改为无绑定。
    /// </summary>
    public static void SetKey(Action action, KeyCode code)
    {
        EnsureLoaded();

        // 冲突处理：占用同一键的其他动作改为 None
        if (code != KeyCode.None)
        {
            foreach (KeyValuePair<Action, KeyCode> kv in bindings)
            {
                if (kv.Key != action && kv.Value == code)
                {
                    bindings[kv.Key] = KeyCode.None;
                    Debug.Log($"[GameInputs] {kv.Key} 的按键 {code} 被移除（被 {action} 占用）");
                }
            }
        }

        bindings[action] = code;
        Save();
        OnBindingsChanged?.Invoke();
    }

    /// <summary>
    /// 开始等待改键输入
    /// </summary>
    public static void BeginCapture()
    {
        IsCapturing = true;
    }

    /// <summary>
    /// 结束等待改键输入
    /// </summary>
    public static void EndCapture()
    {
        IsCapturing = false;
    }

    #endregion

    #region ===== 持久化 =====

    private static void EnsureLoaded()
    {
        if (bindings != null) { return; }

        bindings = new Dictionary<Action, KeyCode>(defaults);

        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            BindingSave data = JsonUtility.FromJson<BindingSave>(json);
            if (data?.actions != null && data.keys != null)
            {
                for (int i = 0; i < data.actions.Length && i < data.keys.Length; i++)
                {
                    Action action = (Action)data.actions[i];
                    if (Enum.IsDefined(typeof(Action), action) && bindings.ContainsKey(action))
                    {
                        bindings[action] = (KeyCode)data.keys[i];
                    }
                }
            }
        }
    }

    private static void Save()
    {
        EnsureLoaded();

        List<int> actions = new List<int>();
        List<int> keys = new List<int>();
        foreach (KeyValuePair<Action, KeyCode> kv in bindings)
        {
            actions.Add((int)kv.Key);
            keys.Add((int)kv.Value);
        }

        BindingSave data = new BindingSave
        {
            actions = actions.ToArray(),
            keys = keys.ToArray(),
        };
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    [Serializable]
    private class BindingSave
    {
        public int[] actions;
        public int[] keys;
    }

    #endregion
}
