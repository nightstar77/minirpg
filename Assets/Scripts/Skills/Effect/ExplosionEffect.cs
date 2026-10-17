using UnityEngine;

/// <summary>
/// 爆炸特效：生成后自动在 lifeTime 秒后销毁
/// </summary>
public class ExplosionEffect : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 0.5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}