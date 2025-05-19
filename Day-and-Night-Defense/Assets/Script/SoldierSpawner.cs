using UnityEngine;

public class SoldierSpawner : MonoBehaviour
{
    [Tooltip("스폰하려면 설치되어 있어야 할 타워 타입 ID")]
    public string requiredTowerTypeID;

    public GameObject soldierPrefab;

    public void TrySpawnSoldier(Vector3 position)
    {
        if (!TowerManager.Instance.HasTower(requiredTowerTypeID))
        {
            Debug.Log($"[{requiredTowerTypeID}] 타워가 설치되어야 이 병사를 사용할 수 있습니다.");
            return;
        }
        Instantiate(soldierPrefab, position, Quaternion.identity);
    }
}
