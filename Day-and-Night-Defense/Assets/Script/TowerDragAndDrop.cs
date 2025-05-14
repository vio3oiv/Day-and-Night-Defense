using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Custom/Tower Drag and Drop")]
public class TowerDragAndDrop : MonoBehaviour
{
    [Header("Tower Prefab Settings")]
    public GameObject towerPrefab;
    public GameObject towerIconPrefab;
    public GameObject placeEffectPrefab;

    [Header("Placement Areas")]
    [Tooltip("설치 가능한 모든 영역 콜라이더를 추가하세요.")]
    public Collider2D[] placeableAreas;

    private GameObject currentIcon;
    private SpriteRenderer iconRenderer;
    private bool isPlacing = false;

    void Update()
    {
        if (!isPlacing || currentIcon == null)
            return;

        // 마우스 위치로 아이콘 이동
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        currentIcon.transform.position = worldPos;

        // 설치 가능 여부 & 골드 체크
        bool inAnyArea = IsInAnyPlaceableArea(worldPos);
        bool hasGold = ResourceManager.Instance != null
                        && ResourceManager.Instance.Gold >= GetPlacementCost();
        UpdateIconColor(inAnyArea && hasGold);

        // 클릭하면 설치 시도
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            TryPlaceTower(worldPos);

        // ESC 누르면 취소
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelPlacing();
    }

    public void StartPlacingTower()
    {
        if (isPlacing)
        {
            CancelPlacing();
            return;
        }

        isPlacing = true;
        currentIcon = Instantiate(towerIconPrefab);
        iconRenderer = currentIcon.GetComponent<SpriteRenderer>();
        if (iconRenderer == null)
            Debug.LogWarning("TowerIconPrefab에 SpriteRenderer가 없습니다.");
    }

    private void TryPlaceTower(Vector3 position)
    {
        // 1) ResourceManager 확인
        var rm = ResourceManager.Instance;
        if (rm == null)
        {
            Debug.LogWarning("[TowerDragAndDrop] ResourceManager가 없습니다.");
            return;
        }

        // 2) 비용 차감
        int cost = GetPlacementCost();
        if (!rm.SpendGold(cost))
        {
            Debug.Log("골드가 부족합니다.");
            return;
        }

        // 3) 설치 가능 영역 체크
        if (IsInAnyPlaceableArea(position))
        {
            Instantiate(towerPrefab, position, Quaternion.identity);

            if (placeEffectPrefab != null)
            {
                var fx = Instantiate(placeEffectPrefab, position, Quaternion.identity);
                Destroy(fx, 2f);
            }

            EndPlacing();
        }
        else
        {
            Debug.Log("Cannot place tower here.");
            // (원한다면 여기에 rm.AddGold(cost)를 호출해 비용 환불 처리)
        }
    }

    private void EndPlacing()
    {
        if (currentIcon != null)
            Destroy(currentIcon);
        isPlacing = false;
    }

    private void CancelPlacing()
    {
        Debug.Log("Placement cancelled.");
        EndPlacing();
    }

    private void UpdateIconColor(bool canPlace)
    {
        if (iconRenderer == null) return;
        iconRenderer.color = canPlace
            ? new Color(0f, 1f, 0f, 0.6f)
            : new Color(1f, 0f, 0f, 0.6f);
    }

    // Helper: 여러 영역 중 하나라도 포함되는지 체크
    private bool IsInAnyPlaceableArea(Vector2 pos)
    {
        foreach (var area in placeableAreas)
            if (area != null && area.OverlapPoint(pos))
                return true;
        return false;
    }

    // Helper: towerPrefab에서 placementCost 읽기
    private int GetPlacementCost()
    {
        var towerComp = towerPrefab.GetComponent<Tower>();
        return towerComp != null ? towerComp.placementCost : 0;
    }
}
