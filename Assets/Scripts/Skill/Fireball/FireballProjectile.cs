using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float damage;

    public void Init(Vector2 dir, float dmg, float spd)
    {
        direction = dir.normalized;
        damage = dmg;
        speed = spd;

        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable target = collision.GetComponent<IDamageable>();

        if (target != null)
        {
            target.TakeDamage(damage, direction);
            Destroy(gameObject);
        }

    }
}