using UnityEngine;
public class ExplosionEffect : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 0.5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}