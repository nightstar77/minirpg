using UnityEngine;
using System.Collections.Generic;

public class SkillTreeUIManager : MonoBehaviour
{
    [Header("技能节点数据")]
    public List<SkillNodeData> skillNodes;

    [SerializeField]
    private SkillTreeManager skillTreeManager;

    [Header("UI")]
    public Transform content;
    public GameObject skillNodePrefab;
    private List<SkillNodeUI> nodeUIs = new List<SkillNodeUI>();

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        foreach (SkillNodeData node in skillNodes)
        {
            GameObject obj = Instantiate(skillNodePrefab, content);
            SkillNodeUI ui = obj.GetComponent<SkillNodeUI>();
            ui.Init(node, skillTreeManager);

            nodeUIs.Add(ui);
        }

    }

    public void RefreshAll()
    {
        foreach (SkillNodeUI ui in nodeUIs)
        { ui.Refresh(); }
    }

}