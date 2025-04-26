using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    public GameObject towerPrefab;
    public Transform[] spawnPoints;

    private int currentIndex = 0;  

    public void SpawnNextTower()
    {
        if (currentIndex < spawnPoints.Length)
        {
            Instantiate(towerPrefab, spawnPoints[currentIndex].position, Quaternion.identity);
            currentIndex++;
        }
        else
        {
            Debug.Log("��� Ÿ�� ���� ����Ʈ�� ����߽��ϴ�.");
        }
    }
}
