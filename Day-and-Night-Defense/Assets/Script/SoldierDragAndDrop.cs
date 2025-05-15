using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Custom/Soldier Drag and Drop")]
public class SoldierDragAndDrop : MonoBehaviour
{
    [Header("Soldier Prefab Settings")]
    public GameObject soldierPrefab;
    public GameObject soldierIconPrefab;
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

        // 설치 가능 여부 체크
        bool canPlace = IsInAnyPlaceableArea(worldPos);
        UpdateIconColor(canPlace);

        // 클릭하면 설치 시도
        if (canPlace && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            TryPlaceSoldier(worldPos);

        // ESC 누르면 취소
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelPlacing();
    }

    /// <summary>
    /// 호출 시 병사 배치 모드를 시작합니다.
    /// </summary>
    public void StartPlacingSoldier()
    {
        if (isPlacing)
        {
            CancelPlacing();
            return;
        }

        isPlacing = true;
        currentIcon = Instantiate(soldierIconPrefab);
        iconRenderer = currentIcon.GetComponent<SpriteRenderer>();
        if (iconRenderer == null)
            Debug.LogWarning("SoldierIconPrefab에 SpriteRenderer가 없습니다.");
    }

    private void TryPlaceSoldier(Vector3 position)
    {
        Instantiate(soldierPrefab, position, Quaternion.identity);
        if (placeEffectPrefab != null)
        {
            var fx = Instantiate(placeEffectPrefab, position, Quaternion.identity);
            Destroy(fx, 2f);
        }
        EndPlacing();
    }

    private bool IsInAnyPlaceableArea(Vector2 pos)
    {
        foreach (var area in placeableAreas)
        {
            if (area != null && area.OverlapPoint(pos))
                return true;
        }
        return false;
    }

    private void UpdateIconColor(bool canPlace)
    {
        if (iconRenderer == null)
            return;
        iconRenderer.color = canPlace
            ? new Color(0f, 1f, 0f, 0.6f) // 녹색
            : new Color(1f, 0f, 0f, 0.6f); // 빨강
    }

    private void EndPlacing()
    {
        if (currentIcon != null)
            Destroy(currentIcon);
        isPlacing = false;
    }

    private void CancelPlacing()
    {
        if (currentIcon != null)
            Destroy(currentIcon);
        isPlacing = false;
    }
}
