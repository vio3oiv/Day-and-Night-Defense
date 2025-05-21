using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Custom/Tower Drag and Drop")]
public class TowerDragAndDrop : MonoBehaviour
{
    public static TowerDragAndDrop Instance { get; private set; }

    // 현재 placement 모드인지, 그리고 현재 위치가 유효한지 외부에서 참조 가능
    public static bool PlacementActive => Instance != null && Instance.isPlacing;
    public static bool PlacementValid => Instance != null && Instance.canPlaceCurrent;

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
    public bool IsPlacing => isPlacing;
    private bool canPlaceCurrent = false;

    void Awake()
    {
        // 싱글턴 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!isPlacing || currentIcon == null)
            return;

        // 마우스 위치로 아이콘 이동
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        currentIcon.transform.position = worldPos;

        // 설치 가능 여부 & 골드 체크 & 공간 체크
        bool inArea = IsInAnyPlaceableArea(worldPos);
        bool hasGold = ResourceManager.Instance != null
                         && ResourceManager.Instance.Gold >= GetPlacementCost();
        bool spaceFree = IsSpaceFree(worldPos);

        // 최종 설치 가능 여부 저장
        canPlaceCurrent = inArea && hasGold && spaceFree;
        UpdateIconColor(canPlaceCurrent);

        // 클릭하면 설치 시도
        if (canPlaceCurrent
            && Input.GetMouseButtonDown(0)
            && !EventSystem.current.IsPointerOverGameObject())
        {
            TryPlaceTower(worldPos);
        }

        // ESC 누르면 취소
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelPlacing();
    }

    public void StartPlacingTower()
    {
        if (DayNightManager.Instance != null && DayNightManager.Instance.CurrentPhase != TimePhase.Day)
        {
            Debug.Log("타워는 낮에만 설치할 수 있습니다.");
            return;
        }

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
        // 비용 차감
        var rm = ResourceManager.Instance;
        if (rm == null)
        {
            Debug.LogWarning("[TowerDragAndDrop] ResourceManager가 없습니다.");
            return;
        }

        int cost = GetPlacementCost();
        if (!rm.SpendGold(cost))
        {
            Debug.Log("골드가 부족합니다.");
            return;
        }

        // 타워 생성 및 등록
        var towerGO = Instantiate(towerPrefab, position, Quaternion.identity);
        var towerComp = towerGO.GetComponent<Tower>();
        if (towerComp != null)
            TowerManager.Instance.RegisterTower(towerComp.towerTypeID);

        // 배치 이펙트
        if (placeEffectPrefab != null)
        {
            var fx = Instantiate(placeEffectPrefab, position, Quaternion.identity);
            Destroy(fx, 2f);
        }

        EndPlacing();
    }


    private bool IsSpaceFree(Vector2 pos)
    {
        // 설치된 모든 Tower 오브젝트(TAG: "Tower") 검사
        var towers = GameObject.FindGameObjectsWithTag("Tower");
        foreach (var t in towers)
        {
            var col = t.GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(pos))
                return false;
        }
        return true;
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

    private void UpdateIconColor(bool canPlace)
    {
        if (iconRenderer == null) return;
        iconRenderer.color = canPlace
            ? new Color(0f, 1f, 0f, 0.6f)
            : new Color(1f, 0f, 0f, 0.6f);
    }

    private bool IsInAnyPlaceableArea(Vector2 pos)
    {
        foreach (var area in placeableAreas)
            if (area != null && area.OverlapPoint(pos))
                return true;
        return false;
    }

    private int GetPlacementCost()
    {
        var towerComp = towerPrefab.GetComponent<Tower>();
        return towerComp != null ? towerComp.PlacementCost : 0;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
