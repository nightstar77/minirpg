using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class SkillLineData
{
    public SkillNodeData from;
    public SkillNodeData to;
    public Image line;
}

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
    private List<SkillNodeUI> nodeUIs = new List<SkillNodeUI>();
    private List<SkillLineData> skillLines = new List<SkillLineData>();
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

    public void Generate()
    {
        foreach (SkillNodeData node in skillNodes)
        {
            GameObject obj = Instantiate(skillNodePrefab, nodeParent);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = node.uiPosition;
            SkillNodeUI ui = obj.GetComponent<SkillNodeUI>();
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

    private Image CreateLine(SkillNodeData pre, SkillNodeData node)
    {
        GameObject line = Instantiate(linePrefab, lineParent);

        RectTransform rect = line.GetComponent<RectTransform>();

        Vector3 startPos = nodeMap[pre].transform.position;

        Vector3 endPos = nodeMap[node].transform.position;

        Vector3 dir = endPos - startPos;

        rect.position = (startPos + endPos) / 2;

        rect.sizeDelta = new Vector2(dir.magnitude, rect.sizeDelta.y);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        rect.rotation = Quaternion.Euler(0, 0, angle);

        Image image = line.GetComponent<Image>();

        SkillLineData data = new SkillLineData();

        data.from = pre;
        data.to = node;
        data.line = image;

        skillLines.Add(data);

        return image;
    }

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