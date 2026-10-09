using UnityEngine;
using TMPro;

/// <summary>
/// 技能点 UI：订阅技能点变更事件，实时显示当前技能点数
/// </summary>
public class SkillPointUI : MonoBehaviour
{
    [SerializeField]
    private PlayerSkillPoint skillPoint;

    [SerializeField]
    private TMP_Text text;

    private void Start()
    {
        skillPoint.OnSkillPointChanged += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        text.text = "技能点: " + skillPoint.CurrentPoint;
    }


}