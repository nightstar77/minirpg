using UnityEngine;

/// <summary>
/// 技能释放上下文：传递方向、位置和施法者信息给技能效果
/// </summary>
public class SkillContext
{
    public Vector2 direction;

    public Vector3 position;

    public GameObject caster;
}