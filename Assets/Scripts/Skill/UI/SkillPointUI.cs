using UnityEngine;
using TMPro;

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