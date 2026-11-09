using System.Diagnostics;
using UnityEngine;

namespace MiniRPG
{
    /// <summary>
    /// 统一日志入口。
    /// Debug 级别日志只在编辑器或 Development Build 中输出，发布版会被编译器直接剔除，
    /// 避免调试日志带来的字符串拼接开销与日志噪音。
    /// 错误日志保留到发布版，便于定位线上问题。
    /// </summary>
    public static class Logger
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }

}
