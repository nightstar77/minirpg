using System.Collections.Generic;
using UnityEngine;

namespace MiniRPG
{
    /// <summary>
    /// 可池化对象接口：从池中取出 / 归还池中时回调，用于重置运行时状态。
    /// 实现该接口的组件会在 Spawn 时收到 OnSpawned，Despawn 时收到 OnDespawned。
    /// </summary>
    public interface IPoolable
    {
        void OnSpawned();
        void OnDespawned();
    }

    /// <summary>
    /// 通用对象池（单例，按需自动创建）。
    /// 按预制体分组缓存实例，Spawn/Despawn 替代频繁的 Instantiate/Destroy，降低 GC 峰值。
    ///
    /// 使用方式（代码里直接调用，无需在场景中手动挂载）：
    ///   GameObject go = ObjectPool.Spawn(prefab, pos, rot);
    ///   ObjectPool.Despawn(go);
    ///
    /// 生命周期：对象池挂在当前场景，场景切换时随场景销毁，池内实例一并销毁；
    /// 下一个场景首次调用时自动重建。
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        private static ObjectPool instance;

        public static ObjectPool Instance
        {
            get
            {
                if (instance == null)
                {
                    // 场景中没有池对象时自动创建
                    GameObject go = new GameObject("[ObjectPool]");
                    instance = go.AddComponent<ObjectPool>();
                }
                return instance;
            }
            private set => instance = value;
        }

        // 预制体 → 空闲实例栈
        private readonly Dictionary<GameObject, Stack<GameObject>> pools =
            new Dictionary<GameObject, Stack<GameObject>>();

        // 实例 → 来源预制体（归还时据此找到对应池）
        private readonly Dictionary<GameObject, GameObject> instancePrefab =
            new Dictionary<GameObject, GameObject>();

        // 已在池中的实例，避免重复归还
        private readonly HashSet<GameObject> inPool = new HashSet<GameObject>();

        // 池内实例的统一父节点，保持层级整洁
        private Transform poolRoot;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            poolRoot = new GameObject("[PooledObjects]").transform;
            poolRoot.SetParent(transform, false);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        #region ===== 对外接口 =====

        /// <summary>
        /// 从池中取一个实例（池为空则新建），并激活到指定位置。
        /// </summary>
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null) { return null; }
            return Instance.SpawnInternal(prefab, position, rotation);
        }

        /// <summary>
        /// 把实例归还到池中（隐藏并缓存），下次 Spawn 复用。
        /// </summary>
        public static void Despawn(GameObject instance)
        {
            if (instance == null) { return; }
            Instance.DespawnInternal(instance);
        }

        #endregion

        #region ===== 内部实现 =====

        private GameObject SpawnInternal(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!pools.TryGetValue(prefab, out Stack<GameObject> stack))
            {
                stack = new Stack<GameObject>();
                pools[prefab] = stack;
            }

            // 取出可用实例（场景切换后旧实例会变成 null，跳过即可）
            GameObject go = null;
            while (stack.Count > 0)
            {
                GameObject cached = stack.Pop();
                inPool.Remove(cached);
                if (cached != null)
                {
                    go = cached;
                    break;
                }
            }

            if (go == null)
            {
                go = Instantiate(prefab, poolRoot);
                instancePrefab[go] = prefab;
            }

            go.transform.SetParent(poolRoot, false);
            go.transform.SetPositionAndRotation(position, rotation);
            go.SetActive(true);

            IPoolable poolable = go.GetComponent<IPoolable>();
            if (poolable != null)
            {
                poolable.OnSpawned();
            }
            return go;
        }

        private void DespawnInternal(GameObject go)
        {
            if (go == null || inPool.Contains(go)) { return; }

            IPoolable poolable = go.GetComponent<IPoolable>();
            if (poolable != null)
            {
                poolable.OnDespawned();
            }

            go.SetActive(false);
            if (poolRoot != null)
            {
                go.transform.SetParent(poolRoot, false);
            }

            // 来源预制体已丢失（如资源被卸载）则直接销毁，不再入池
            if (!instancePrefab.TryGetValue(go, out GameObject prefab) || prefab == null)
            {
                Destroy(go);
                return;
            }

            if (!pools.TryGetValue(prefab, out Stack<GameObject> stack))
            {
                stack = new Stack<GameObject>();
                pools[prefab] = stack;
            }
            stack.Push(go);
            inPool.Add(go);
        }

        #endregion
    }
}
