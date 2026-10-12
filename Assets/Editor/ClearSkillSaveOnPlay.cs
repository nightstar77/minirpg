#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// 编辑器工具：进入 Play 模式前自动清除技能存档（PlayerPrefs 中的 "PlayerSkills" 键）
///
/// 目的：
/// 技能解锁记录通过 PlayerPrefs 持久化，之前测试时解锁的技能会一直残留，
/// 导致每次进 Play 模式技能树都是"全解锁"状态。
/// 此脚本在每次点 Play 时自动清掉这份存档，保证测试从干净状态开始。
///
/// 注意：
/// - 仅在编辑器中生效（#if UNITY_EDITOR），打包后的游戏存档不受任何影响
/// - 本文件必须放在 Assets/Editor/ 目录下（Unity 规定编辑器脚本的存放位置）
/// - 如果以后想测试"存档持久化"功能，注释掉 OnPlayModeStateChanged
///   里的清除代码即可
/// </summary>
[InitializeOnLoad]
public static class ClearSkillSaveOnPlay
{
    // [InitializeOnLoad]：编辑器加载时执行静态构造函数，注册回调
    static ClearSkillSaveOnPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // ExitingEditMode：即将离开编辑模式进入 Play 模式
        // 选这个时机是为了在场景 Awake/Load 之前就把存档清掉
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (PlayerPrefs.HasKey("PlayerSkills"))
            {
                PlayerPrefs.DeleteKey("PlayerSkills");
                PlayerPrefs.Save();
                Debug.Log("[编辑器] 进入 Play 模式，已自动清除技能存档");
            }
        }
    }
}
#endif
