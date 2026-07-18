using UnityEngine;
public class PlayerState : MonoBehaviour
{
    #region ===== State Enum =====
    public enum State
    {
        Idle,
        Move,
        Attack,
        Hurt,
        Dead
    }

    #endregion

    #region ===== Runtime =====
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
        if (currentState == newState)
        {
            return;
        }
        Debug.Log($"Player State : {currentState} -> {newState}");
        currentState = newState;
    }

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