using UnityEngine;

/// <summary>
/// 动画事件桥接器：将 Animator 中的 Animation Event 回调转发给 PlayerAttack。
/// 挂载在玩家角色上，由动画剪辑中的 Event 触发。
/// </summary>
public class AttackAnimationEvent : MonoBehaviour
{

    #region ===== Component =====

    private PlayerAttack playerAttack;

    #endregion


    #region ===== Unity =====

    private void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
    }

    #endregion


    #region ===== Animation Event =====


    /// <summary>
    /// 动画攻击开始事件
    ///
    /// 由Animation Event调用
    /// 
    /// 例如：
    /// Attack1挥剑瞬间
    /// </summary>
    public void AttackStart()
    {
        playerAttack.StartHitBox();
    }


    /// <summary>
    /// 动画攻击结束事件
    ///
    /// 例如：
    /// 剑挥完
    /// </summary>
    public void AttackEnd()
    {
        playerAttack.EndHitBox();
    }


    /// <summary>
    /// 连击窗口开启
    ///
    /// 允许玩家输入下一段攻击
    /// </summary>
    public void EnableCombo()
    {
        playerAttack.EnableCombo();
    }


    #endregion

}