/// <summary>
/// 技能树节点状态：用于 UI 颜色和可交互性判断
/// </summary>
public enum SkillNodeState
{
    Locked,              // 未解锁：可点击学习
    Unlocked,            // 已解锁
    NeedPrerequisite,    // 缺少前置技能
    NeedPoint            // 技能点不足
}