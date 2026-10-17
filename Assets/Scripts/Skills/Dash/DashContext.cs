using UnityEngine;

/// <summary>
/// 冲刺参数结构体：传递冲刺方向、速度和持续时长给 PlayerController
/// </summary>
public struct DashContext
{
    public Vector2 direction;
    public float speed;
    public float duration;

    public DashContext(Vector2 direction, float speed, float duration)
    {
        this.direction = direction;
        this.speed = speed;
        this.duration = duration;
    }
}