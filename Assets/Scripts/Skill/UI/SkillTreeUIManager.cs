using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// 技能树连线数据：记录前置节点、后置节点和连线 Image
/// </summary>
[System.Serializable]
public class SkillLineData
{
    public SkillNodeData from;
    public SkillNodeData to;
    public Image line;
}

/// <summary>
/// 技能树 UI 管理器：根据节点数据生成 UI 节点和连线，解锁后刷新显示
/// </summary>
public class SkillTreeUIManager : MonoBehaviour
{
    [Header("技能节点数据")]
    public List<SkillNodeData> skillNodes;

    [SerializeField]
    private SkillTreeManager skillTreeManager;

    [Header("UI")]
    public Transform nodeParent;
    public Transform lineParent;
    public GameObject skillNodePrefab;

    [Header("提示框")]
    [Tooltip("技能描述框相对于技能图标的偏移（向下为负 Y）。所有动态生成的节点统一使用此偏移。")]
    public Vector2 tooltipOffset = new Vector2(0, -80f);

    private List<SkillNodeUI> nodeUIs = new List<SkillNodeUI>();
    private List<SkillLineData> skillLines = new List<SkillLineData>();
    // 节点数据 → UI 映射，用于查找连线端点
    private Dictionary<SkillNodeData, SkillNodeUI> nodeMap = new Dictionary<SkillNodeData, SkillNodeUI>();
    public GameObject SkillTreePanel;

    [SerializeField]
    private GameObject linePrefab;

    private void Start()
    {
        skillTreeManager.OnSkillUnlocked += RefreshAll;
        skillTreeManager.OnSkillUnlocked += RefreshLines;
        Generate();
    }

    /// <summary>
    /// 生成所有节点 UI 并创建连线
    /// </summary>
    public void Generate()
    {
        foreach (SkillNodeData node in skillNodes)
        {
            GameObject obj = Instantiate(skillNodePrefab, nodeParent);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = node.uiPosition;
            SkillNodeUI ui = obj.GetComponent<SkillNodeUI>();
            // 统一设置提示框偏移，避免每个节点单独配置
            ui.tooltipOffset = tooltipOffset;
            ui.Init(node, skillTreeManager);

            nodeUIs.Add(ui);
            nodeMap.Add(node, ui);
        }
        CreateAllLines();
        RefreshLines();
    }

    public void RefreshAll()
    {
        foreach (SkillNodeUI ui in nodeUIs)
        { ui.Refresh(); }
    }

    /// <summary>
    /// 遍历所有节点的前置关系，创建连线
    /// </summary>
    private void CreateAllLines()
    {
        foreach (SkillNodeData node in skillNodes)
        {
            foreach (SkillNodeData pre in node.prerequisites)
            {
                if (nodeMap.ContainsKey(pre))
                {
                    CreateLine(pre, node);
                }
            }
        }
    }

    /// <summary>
    /// 在两个节点之间创建连线 Image，按距离和角度旋转
    /// </summary>
    private Image CreateLine(SkillNodeData pre, SkillNodeData node)
    {
        GameObject line = Instantiate(linePrefab, lineParent);

        RectTransform rect = line.GetComponent<RectTransform>();

        Vector3 startPos = nodeMap[pre].transform.position;
        Vector3 endPos = nodeMap[node].transform.position;
        Vector3 dir = endPos - startPos;

        // 连线中心位置、长度和旋转角度
        rect.position = (startPos + endPos) / 2;
        rect.sizeDelta = new Vector2(dir.magnitude, rect.sizeDelta.y);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rect.rotation = Quaternion.Euler(0, 0, angle);

        Image image = line.GetComponent<Image>();
        // 连线只用于显示，必须关闭射线接收，否则连线端点压在节点图标上会挡住节点的点击/悬停
        image.raycastTarget = false;

        SkillLineData data = new SkillLineData();
        data.from = pre;
        data.to = node;
        data.line = image;

        skillLines.Add(data);

        return image;
    }

    /// <summary>
    /// 根据前置节点是否已解锁，更新连线颜色
    /// </summary>
    public void RefreshLines()
    {
        foreach (SkillLineData lineData in skillLines)
        {
            if (skillTreeManager.HasSkill(lineData.from.skill.skillType))
            {
                lineData.line.color = Color.green;
            }
            else
            {
                lineData.line.color = Color.gray;
            }
        }
    }

    public void ClosePanel()
    {
        SkillTreePanel.SetActive(false);
    }

}