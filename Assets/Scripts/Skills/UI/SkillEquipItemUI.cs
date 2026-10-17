using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// 技能装备选项：显示技能图标和名称，点击触发装备回调。
/// 由 SkillEquipPanelUI 动态生成。
/// </summary>
public class SkillEquipItemUI : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TMP_Text nameText;
    [SerializeField]
    private Button button;

    public void Init(Sprite sprite, string skillName, UnityAction onClick)
    {
        icon.sprite = sprite;
        nameText.text = skillName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);
    }
}
