using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    public GameObject towerPrefab;
    public Transform[] spawnPoints;

    private int currentIndex = 0;  // 다음 타워를 생성할 인덱스

    public void SpawnNextTower()
    {
        if (currentIndex < spawnPoints.Length)
        {
            Instantiate(towerPrefab, spawnPoints[currentIndex].position, Quaternion.identity);
            currentIndex++;
        }
        else
        {
            Debug.Log("모든 타워 스폰 포인트를 사용했습니다.");
        }
    }
}
