using UnityEngine;

/// <summary>
/// ===========================================================
/// PlayerState
///
/// 玩家状态模块
///
/// 【职责】
///
/// 1、记录玩家当前状态
/// 2、切换玩家状态
/// 3、提供状态查询接口
///
/// 【不负责】
///
/// × 播放动画
/// × 玩家移动
/// × 玩家攻击
/// × 血量计算
///
/// Version 0.5
/// ===========================================================
/// </summary>
public class PlayerState : MonoBehaviour
{
    #region ===== State Enum =====

    /// <summary>
    /// 玩家状态枚举。
    ///
    /// enum（Enumeration，枚举）
    /// 用于定义一组固定的状态。
    /// </summary>
    public enum State
    {
        /// <summary>
        /// 待机状态。
        /// </summary>
        Idle,

        /// <summary>
        /// 移动状态。
        /// </summary>
        Move,

        /// <summary>
        /// 攻击状态。
        /// </summary>
        Attack,

        /// <summary>
        /// 死亡状态。
        /// </summary>
        Dead
    }

    #endregion

    #region ===== Runtime =====

    /// <summary>
    /// 当前玩家状态。
    ///
    /// private：
    /// 外部不能直接修改，
    /// 必须通过 ChangeState() 进行切换。
    /// </summary>
    private State currentState = State.Idle;

    #endregion

    #region ===== Public =====

    /// <summary>
    /// 修改玩家状态。
    ///
    /// 参数：
    /// newState —— 要切换到的新状态。
    ///
    /// 为什么要封装？
    ///
    /// 以后：
    /// 切换状态时，
    /// 可以统一播放音效、
    /// 输出日志、
    /// 发送事件。
    /// </summary>
    public void ChangeState(State newState)
    {
        // 如果状态没有变化，就直接返回。
        if (currentState == newState)
        {
            return;
        }

        // （调试用）打印状态变化。
        Debug.Log($"Player State : {currentState} -> {newState}");

        // 更新当前状态。
        currentState = newState;
    }

    /// <summary>
    /// 查询当前是否处于指定状态。
    ///
    /// 参数：
    /// state —— 要判断的状态。
    ///
    /// 返回值：
    /// true  —— 当前就是这个状态。
    /// false —— 当前不是这个状态。
    /// </summary>
    public bool IsState(State state)
    {
        return currentState == state;
    }

    #endregion

    #region ===== Property =====

    /// <summary>
    /// 获取当前状态（只读）。
    ///
    /// 外部可以读取，
    /// 但不能直接修改。
    /// </summary>
    public State CurrentState => currentState;

    #endregion
}