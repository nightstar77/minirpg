using UnityEngine;

/// <summary>
/// 火球投射体：直线飞行，命中敌人后触发所有挂载的技能效果并销毁
/// </summary>
public class FireballProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private bool initialized;

    private SkillEffect[] effects;

    /// <summary>
    /// 初始化飞行方向、速度、生命周期和携带的技能效果
    /// </summary>
    public void Init(Vector2 dir, float spd, float lifeTime, SkillEffect[] skillEffects)
    {
        initialized = true;
        direction = dir.normalized;
        speed = spd;

        effects = skillEffects;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (!initialized) { return; }
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 仅碰撞 Enemy 层
        if (collision.gameObject.layer != LayerMask.NameToLayer("Enemy")) { return; }
        IDamageable target = collision.GetComponentInParent<IDamageable>();

        if (target != null)
        {
            // 构造技能上下文，依次应用所有效果（伤害、减速等）
            SkillContext context = new SkillContext();
            context.direction = direction;
            context.position = transform.position;
            context.caster = gameObject;

            foreach (SkillEffect effect in effects)
            {
                effect.Apply(target, context);
            }
            Destroy(gameObject);
        }

    }
}