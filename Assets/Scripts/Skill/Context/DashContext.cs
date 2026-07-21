using UnityEngine;

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