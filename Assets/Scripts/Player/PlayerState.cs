using UnityEngine;

/// <summary>
/// 玩家状态管理：封装状态切换与查询，供其他脚本统一读写
/// </summary>
public class PlayerState : MonoBehaviour
{
    #region ===== State Enum =====
    public enum State { Idle, Move, Attack, Hurt, Defend, Dead }

    #endregion

    #region ===== Runtime =====
    private State currentState = State.Idle;

    #endregion

    #region ===== Public =====

    /// <summary>
    /// 切换玩家状态（相同状态不重复切换）
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
    /// 当前状态（只读），外部可读不可写
    /// </summary>
    public State CurrentState => currentState;

    #endregion
}