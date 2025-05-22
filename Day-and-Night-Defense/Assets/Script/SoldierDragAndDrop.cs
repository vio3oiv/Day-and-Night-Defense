using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Custom/Soldier Drag and Drop")]
public class SoldierDragAndDrop : MonoBehaviour
{
    public static SoldierDragAndDrop Instance { get; private set; }

    // 외부에서 읽기 전용으로 배치 모드 여부 조회
    public bool IsPlacing => isPlacing;
    private bool isPlacing = false;

    [Header("Soldier Prefab Settings")]
    public GameObject soldierPrefab;
    public GameObject soldierIconPrefab;
    public GameObject placeEffectPrefab;

    [Header("Placement Cost")]
    public int buildCost = 50;

    [Header("Placement Areas")]
    [Tooltip("설치 가능한 모든 영역 콜라이더를 추가하세요.")]
    public Collider2D[] placeableAreas;

    private GameObject currentIcon;
    private SpriteRenderer iconRenderer;

    void Awake()
    {
        // 싱글턴 초기화
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (!isPlacing || currentIcon == null) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        currentIcon.transform.position = worldPos;

        // 1) 골드 체크
        bool hasGold = ResourceManager.Instance.CurrentGold >= buildCost;

        // 2) 설치 가능 여부
        bool canPlace =
            IsInAnyPlaceableArea(worldPos) &&
            IsSpaceFree(worldPos) &&
            hasGold;

        // **아이콘 색상 갱신**
        UpdateIconColor(canPlace);

        // 설치 시도
        if (canPlace
            && Input.GetMouseButtonDown(0)
            && !EventSystem.current.IsPointerOverGameObject())
        {
            TryPlaceSoldier(worldPos);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelPlacing();
    }


    /// <summary>
    /// 병사 배치 모드를 시작합니다.
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
        // 최종 체크
        if (!IsInAnyPlaceableArea(position)
            || !IsSpaceFree(position)
            || ResourceManager.Instance.CurrentGold < buildCost)
            return;

        // **골드 먼저 차감**
        ResourceManager.Instance.SpendGold(buildCost);

        // 실제 배치
        Instantiate(soldierPrefab, position, Quaternion.identity);

        if (placeEffectPrefab != null)
        {
            var fx = Instantiate(placeEffectPrefab, position, Quaternion.identity);
            Destroy(fx, 2f);
        }

        EndPlacing();
    }

    /// <summary>지정된 영역 중 하나라도 포함되는지 체크</summary>
    private bool IsInAnyPlaceableArea(Vector2 pos)
    {
        foreach (var area in placeableAreas)
            if (area != null && area.OverlapPoint(pos))
                return true;
        return false;
    }

    /// <summary>같은 위치에 이미 설치된 Soldier가 있는지 체크</summary>
    private bool IsSpaceFree(Vector2 pos)
    {
        var col = soldierPrefab.GetComponent<BoxCollider2D>();
        if (col == null) return true;

        // 설치할 칸 주변의 Soldier만 세서, 자신 포함 최대 1개일 때만 설치 허용
        var hits = Physics2D.OverlapBoxAll(
            pos + col.offset,
            col.size,
            0f
        );
        int soldierCount = 0;
        foreach (var h in hits)
            if (h.CompareTag("Soldier"))
                soldierCount++;
        return soldierCount <= 1;
    }

    private void UpdateIconColor(bool canPlace)
    {
        if (iconRenderer == null) return;
        iconRenderer.color = canPlace
            ? new Color(0f, 1f, 0f, 0.6f)  // 녹색
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
