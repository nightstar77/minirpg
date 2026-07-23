using UnityEngine;

public class SkillTreeTest : MonoBehaviour
{
    public SkillNodeData fireballNode;
    private SkillTreeManager tree;

    private void Awake()
    {
        tree = GetComponent<SkillTreeManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            tree.UnlockSkill(fireballNode);
        }
    }

}