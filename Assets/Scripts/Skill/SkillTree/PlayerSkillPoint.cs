using UnityEngine;

public class PlayerSkillPoint : MonoBehaviour
{
    [Header("技能点")]
    [SerializeField]
    private int skillPoints = 5;

    public bool CanSpend(int cost)
    {
        return skillPoints >= cost;
    }

    public bool SpendPoint(int amount)
    {
        if (skillPoints < amount)
        {
            Debug.Log("技能点不足");
            return false;
        }
        skillPoints -= amount;

        Debug.Log("消耗技能点:" + amount + " 剩余:" + skillPoints);
        return true;
    }

    public void AddPoint(int amount)
    {
        skillPoints += amount;
        Debug.Log("获得技能点:" + amount);
    }
}