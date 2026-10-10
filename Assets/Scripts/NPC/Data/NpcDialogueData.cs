using UnityEngine;

namespace NPC.Data
{
    /// <summary>
    /// NPC 对话数据（ScriptableObject）
    /// 设计思路参考 SkillData：把对话内容做成可配置的数据资产，
    /// 在 Project 窗口右键 Create → NPC → Npc Dialogue Data 创建
    /// 同一个 NPC 可以根据任务进度切换不同的对话数据
    /// </summary>
    [CreateAssetMenu(fileName = "NewNpcDialogue", menuName = "NPC/Npc Dialogue Data")]
    public class NpcDialogueData : ScriptableObject
    {
        [Header("基础信息")]
        [Tooltip("NPC 名称，显示在对话框顶部")]
        public string npcName = "NPC";

        [Tooltip("NPC 头像，显示在对话框左侧（可选，不填则不显示）")]
        public Sprite npcPortrait;

        [Header("对话内容")]
        [Tooltip("对话行数组，按顺序逐行显示")]
        [TextArea(3, 6)]
        public string[] dialogueLines;

        [Header("可选配置")]
        [Tooltip("对话结束后是否触发事件（用于任务系统扩展）")]
        public bool triggerEventOnEnd = false;

        [Tooltip("事件 ID，供外部系统（如任务系统）订阅判断")]
        public int eventId = 0;
    }
}
