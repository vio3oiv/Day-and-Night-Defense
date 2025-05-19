using UnityEngine;

public class SoldierSpawner : MonoBehaviour
{
    [Tooltip("�����Ϸ��� ��ġ�Ǿ� �־�� �� Ÿ�� Ÿ�� ID")]
    public string requiredTowerTypeID;

    public GameObject soldierPrefab;

    public void TrySpawnSoldier(Vector3 position)
    {
        if (!TowerManager.Instance.HasTower(requiredTowerTypeID))
        {
            Debug.Log($"[{requiredTowerTypeID}] Ÿ���� ��ġ�Ǿ�� �� ���縦 ����� �� �ֽ��ϴ�.");
            return;
        }
        Instantiate(soldierPrefab, position, Quaternion.identity);
    }
}
