using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("몬스터 설정")]
    public GameObject monsterPrefab;
    public Transform[] spawnPoints;

    [Header("스폰 설정")]
    public float spawnDelayMin = 1f;
    public float spawnDelayMax = 3f;
    public float startDelay = 1f;
    public bool autoSpawn = true;

    void Start()
    {
        if (autoSpawn)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            SpawnMonster();
            float delay = Random.Range(spawnDelayMin, spawnDelayMax);
            yield return new WaitForSeconds(delay);
        }
    }

    public void SpawnMonster()
    {
        if (spawnPoints.Length == 0 || monsterPrefab == null) return;

        int randIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randIndex];

        Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);
    }
}
