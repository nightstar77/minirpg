using UnityEngine;

/// <summary>
/// 技能输入处理：监听技能槽按键（默认 Q/E/R/F，可在设置面板改键），
/// 将按键映射到对应技能槽并触发技能
/// </summary>
public class SkillInput : MonoBehaviour
{
    [SerializeField]
    private SkillController skillController;

    private void Awake()
    {
        if (skillController == null)
        { skillController = GetComponent<SkillController>(); }
    }

    private void Update()
    {
        // 改键捕获期间暂停游戏输入
        if (GameInputs.IsCapturing)
        {
            return;
        }

        // 按槽位顺序读取各自绑定的按键
        for (int i = 0; i < GameInputs.SkillActions.Length; i++)
        {
            if (Input.GetKeyDown(GameInputs.GetSkillKey(i)))
            {
                UseSkillSlot(i);
            }
        }
    }

    private void UseSkillSlot(int index)
    {
        // 槽位索引与 GameInputs.SkillActions 一一对应，直接使用
        skillController.UseSkill(index);
    }

}