using UnityEngine;

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