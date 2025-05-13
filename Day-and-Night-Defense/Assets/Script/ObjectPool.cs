using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // Ǯ���� ������ ����
    [System.Serializable]
    public class Pool
    {
        public string tag;        // �ĺ��� �±�
        public GameObject prefab;     // Ǯ���� ������
        public int size;       // �ʱ� Ǯ ũ��
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

    // Ǯ���� ������
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
        poolDictionary[tag].Enqueue(obj);  // ��ȯ ť
        return obj;
    }
}
