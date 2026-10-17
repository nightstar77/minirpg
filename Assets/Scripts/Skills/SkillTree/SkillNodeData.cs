using UnityEngine;

/// <summary>
/// 技能树节点配置：定义技能、解锁消耗、前置条件和在技能树面板中的位置
/// </summary>
[CreateAssetMenu(fileName = "SkillNode", menuName = "Game/Skill/Skill Node")]
public class SkillNodeData : ScriptableObject
{
    [Header("技能配置")]
    public SkillData skill;

    [Header("解锁条件")]
    public int cost = 1;

    [Header("前置技能")]
    public SkillNodeData[] prerequisites;

    [Header("UI位置")]
    public Vector2 uiPosition;

}