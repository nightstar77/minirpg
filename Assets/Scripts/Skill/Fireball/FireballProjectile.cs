using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private bool initialized;

    private SkillEffect[] effects;

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
        if (collision.gameObject.layer != LayerMask.NameToLayer("Enemy")) { return; }
        IDamageable target = collision.GetComponentInParent<IDamageable>();

        if (target != null)
        {
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