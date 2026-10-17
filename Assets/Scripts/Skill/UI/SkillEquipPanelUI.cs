using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 技能装备选择面板：点击技能槽后弹出，列出所有已解锁技能供选择装备。
/// 单例模式，技能槽通过 Instance.Open 调用。
/// </summary>
public class SkillEquipPanelUI : MonoBehaviour
{
    public static SkillEquipPanelUI Instance { get; private set; }

    [Header("UI")]
    [Tooltip("技能选项的父物体（挂 Vertical Layout Group 自动排列）")]
    [SerializeField]
    private Transform itemParent;

    [Tooltip("技能选项预制体（挂 SkillEquipItemUI）")]
    [SerializeField]
    private SkillEquipItemUI itemPrefab;

    [SerializeField]
    private Button closeButton;

    // 当前要装备到的槽位
    private int targetSlot;
    private SkillController skillController;
    private PlayerSkillInventory inventory;
    // 已生成的选项，打开时先清空再生成
    private List<SkillEquipItemUI> items = new List<SkillEquipItemUI>();

    private void Awake()
    {
        Instance = this;

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }

        // 做完初始化再隐藏（挂在未激活物体上 Awake 不会执行）
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 打开面板：列出所有已解锁技能，点击即装备到指定槽位
    /// </summary>
    public void Open(int slotIndex, SkillController controller)
    {
        targetSlot = slotIndex;
        skillController = controller;
        inventory = controller.GetComponent<PlayerSkillInventory>();

        Populate();
        gameObject.SetActive(true);
        Debug.Log("[SkillEquipPanel] 打开面板，目标槽位: " + slotIndex);
    }

    /// <summary>
    /// 清空旧选项，按当前已解锁技能重新生成
    /// </summary>
    private void Populate()
    {
        foreach (SkillEquipItemUI item in items)
        {
            Destroy(item.gameObject);
        }
        items.Clear();

        foreach (SkillType type in inventory.GetSkills())
        {
            SkillBase skill = skillController.GetSkill(type);
            if (skill == null) { continue; }

            SkillEquipItemUI item = Instantiate(itemPrefab, itemParent);
            SkillType captured = type; // 闭包捕获，避免全部指向最后一个
            item.Init(skill.SkillData.icon, skill.SkillData.skillName, () => Equip(captured));
            items.Add(item);
        }
    }

    private void Equip(SkillType type)
    {
        if (skillController.EquipSkill(targetSlot, type))
        {
            Close();
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
