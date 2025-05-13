using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // 풀마다 관리할 정보
    [System.Serializable]
    public class Pool
    {
        public string tag;        // 식별용 태그
        public GameObject prefab;     // 풀링할 프리팹
        public int size;       // 초기 풀 크기
    }

    public static ObjectPool Instance { get; private set; }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                var obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, queue);
        }
    }

    // 풀에서 꺼내기
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"[ObjectPool] Pool with tag '{tag}' doesn't exist.");
            return null;
        }

        var obj = poolDictionary[tag].Dequeue();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        poolDictionary[tag].Enqueue(obj);  // 순환 큐
        return obj;
    }
}
