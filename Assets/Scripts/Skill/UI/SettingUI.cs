using UnityEngine;

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
