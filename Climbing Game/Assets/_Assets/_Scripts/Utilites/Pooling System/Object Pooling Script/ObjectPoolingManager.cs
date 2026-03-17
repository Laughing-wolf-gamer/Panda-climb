using UnityEngine;
using System.Collections.Generic;

namespace GamerWolf.Utils
{
    [DefaultExecutionOrder(-1)]
    public class ObjectPoolingManager : MonoBehaviour
    {
        private class PoolRuntime
        {
            public PoolSO config;
            public Queue<GameObject> objects = new Queue<GameObject>();
            public Transform parent;
        }

        public static ObjectPoolingManager current { get; private set; }

        [SerializeField] private List<PoolSO> pools = new List<PoolSO>();
        private Dictionary<string, PoolRuntime> poolDictionary;

        private void Awake()
        {
            if (current == null)
            {
                current = this;
            }
            else
            {
                Debug.LogError(nameof(ObjectPoolingManager) + " is Found in the Scene");
                Destroy(current.gameObject);
                return;
            }

            Init();
        }

        private void Init()
        {
            poolDictionary = new Dictionary<string, PoolRuntime>();
            CreatePool();
        }

        public void CreatePool()
        {
            foreach (PoolSO pool in pools)
            {
                if (pool == null || pool.prefabs == null || string.IsNullOrEmpty(pool.tag))
                {
                    Debug.LogWarning("Skipping invalid pool config.");
                    continue;
                }

                if (poolDictionary.ContainsKey(pool.tag))
                {
                    Debug.LogWarning("Pool with tag " + pool.tag + " already exists.");
                    continue;
                }

                GameObject parentObject = new GameObject(pool.tag + " Pooled Object Parent");
                parentObject.transform.SetParent(transform);

                PoolRuntime runtime = new PoolRuntime
                {
                    config = pool,
                    parent = parentObject.transform
                };

                AddObjectsToPool(runtime, pool.size);
                poolDictionary.Add(pool.tag, runtime);
            }
        }

        private void AddObjectsToPool(PoolRuntime runtime, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject obj = Instantiate(runtime.config.prefabs, runtime.parent);
                obj.SetActive(false);
                obj.name = runtime.config.tag + " " + runtime.objects.Count;
                runtime.objects.Enqueue(obj);
            }
        }

        private bool TryGetAvailableObject(PoolRuntime runtime, out GameObject availableObject)
        {
            availableObject = null;
            int count = runtime.objects.Count;

            for (int i = 0; i < count; i++)
            {
                GameObject candidate = runtime.objects.Dequeue();
                runtime.objects.Enqueue(candidate);

                if (availableObject == null && !candidate.activeInHierarchy)
                {
                    availableObject = candidate;
                }
            }

            if (availableObject != null)
            {
                return true;
            }

            if (!runtime.config.canExpand)
            {
                return false;
            }

            int amountToAdd = runtime.config.expandAmount;

            if (runtime.config.maxSize > 0)
            {
                int remainingSpace = runtime.config.maxSize - runtime.objects.Count;
                amountToAdd = Mathf.Min(amountToAdd, remainingSpace);
            }

            if (amountToAdd <= 0)
            {
                return false;
            }

            AddObjectsToPool(runtime, amountToAdd);

            return TryGetAvailableObject(runtime, out availableObject);
        }

        private string GetRandomTag()
        {
            int randomNum = Random.Range(0, pools.Count);
            return pools[randomNum].tag;
        }

        public GameObject SpawnRandomFromPool(Vector3 spawnPoint, Quaternion rotation)
        {
            return SpawnFromPool(GetRandomTag(), spawnPoint, rotation);
        }

        public GameObject SpawnFromPool(string tag)
        {
            return SpawnFromPool(tag, Vector3.zero, Quaternion.identity);
        }

        public GameObject SpawnFromPool(string tag, Vector3 spawnPosition, Quaternion rotation, Transform parent)
        {
            GameObject newObject = SpawnFromPool(tag, spawnPosition, rotation);
            if (newObject != null)
            {
                newObject.transform.SetParent(parent);
            }

            return newObject;
        }

        public GameObject SpawnFromPool(string tag, Vector3 spawnPosition, Quaternion rotation)
        {
            if (!poolDictionary.TryGetValue(tag, out PoolRuntime runtime))
            {
                Debug.LogWarning("Pool With the tag " + tag + " is not Found");
                return null;
            }

            if (!TryGetAvailableObject(runtime, out GameObject objectToSpawn))
            {
                Debug.LogWarning("Pool " + tag + " has no available objects and cannot expand anymore.");
                return null;
            }

            objectToSpawn.transform.SetPositionAndRotation(spawnPosition, rotation);
            objectToSpawn.SetActive(true);

            IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();
            if (pooledObject != null)
            {
                pooledObject.OnObjectReuse();
            }

            return objectToSpawn;
        }
    }
}
