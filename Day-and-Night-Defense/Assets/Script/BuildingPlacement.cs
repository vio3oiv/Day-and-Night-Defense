using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingPlacement : MonoBehaviour
{
    public Tilemap gridMap;
    public GameObject buildingPrefab;
    public int buildingCost = 50;

    void Update()
    {
        if (GamePhaseManager.Instance.CurrentPhase != Phase.Build) return;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = gridMap.WorldToCell(worldPos);

            if (ResourceManager.Instance.SpendGold(buildingCost))
            {
                Instantiate(buildingPrefab, gridMap.GetCellCenterWorld(cellPos), Quaternion.identity);
            }
        }
    }
}
