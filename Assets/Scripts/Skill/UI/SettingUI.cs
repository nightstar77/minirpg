using UnityEngine;

/// <summary>
/// 设置面板控制：打开技能树面板
/// </summary>
public class SettingUI : MonoBehaviour
{
    public GameObject SkillTreePanel;
    void Start()
    {
        SkillTreePanel.SetActive(false);
    }
    public void SettingOpen()
    {
        SkillTreePanel.SetActive(true);
    }
}
