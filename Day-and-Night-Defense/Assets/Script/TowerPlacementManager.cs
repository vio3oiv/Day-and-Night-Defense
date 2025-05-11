using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 낮 페이즈에 타워 선택, 설치, 그리고 밤 전환 UI를 관리합니다.
/// 기능:
/// 1. 설치 가능 구역 클릭 → 타워 선택 패널 표시
/// 2. 타워 선택 버튼 클릭 → 설치 모드 진입 (아이콘 마우스 커서로 표시)
/// 3. 설치 위치 클릭 → 골드 차감 후 타워 설치 & 이펙트
/// 4. 낮 메시지 표시 (정해진 시간 후)
/// 5. '밤 시작' 버튼 클릭 → 팝업 → 확인 시 밤 모드 전환
/// </summary>
public class TowerPlacementManager : MonoBehaviour
{
    [Header("설치 영역")]
    [Tooltip("설치 가능한 영역 레이어 마스크")]
    public LayerMask placementLayer;
    [Tooltip("설치 불가능한 영역 검사용 콜라이더(선택)")]
    public Collider2D placeableArea;

    [Header("타워 설정")]
    [Tooltip("설치 가능한 타워 프리팹 배열")] public GameObject[] towerPrefabs;
    [Tooltip("타워별 비용(프리팹 순서와 동일)")] public int[] towerCosts;

    [Header("타워 선택 UI")]
    public GameObject towerSelectPanel;    // 타워 선택 버튼들이 담긴 패널
    public Button[] towerSelectButtons;    // 인덱스별 버튼

    [Header("플레이스 아이콘 & 이펙트")]
    public GameObject towerIconPrefab;
    public GameObject placeEffectPrefab;

    [Header("낮 메시지")]
    public TextMeshProUGUI dayMessageText;

    [Header("밤 전환 UI")]
    public Button startNightButton;
    public GameObject nightPopup;
    public Button nightConfirmButton;
    public Button nightCancelButton;

    [Header("골드 안내")]
    public TextMeshProUGUI insufficientGoldText;

    private Transform selectedArea;
    private GameObject currentIcon;
    private SpriteRenderer iconRenderer;
    private bool isPlacing = false;
    private GameObject selectedPrefab;
    private int selectedCost;

    void Start()
    {
        // 타워 선택 버튼 콜백 연결
        for (int i = 0; i < towerSelectButtons.Length; i++)
        {
            int idx = i;
            towerSelectButtons[i].onClick.AddListener(() => OnTowerButton(idx));
        }
        towerSelectPanel.SetActive(false);

        // 낮 메시지
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged += OnPhaseChanged;

        // 밤 시작 버튼
        startNightButton.onClick.AddListener(() => nightPopup.SetActive(true));
        nightConfirmButton.onClick.AddListener(() => {
            nightPopup.SetActive(false);
            DayNightManager.Instance.SetPhase(TimePhase.Night);
        });
        nightCancelButton.onClick.AddListener(() => nightPopup.SetActive(false));
        nightPopup.SetActive(false);

        insufficientGoldText.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    void OnPhaseChanged(TimePhase phase)
    {
        if (phase == TimePhase.Day)
        {
            // 낮 시작 메시지
            StartCoroutine(DayRoutine());
            // 설치 UI 활성
            startNightButton.gameObject.SetActive(true);
        }
        else
        {
            // 밤 시작 시 설치 모드 해제
            CancelPlacing();
            towerSelectPanel.SetActive(false);
            startNightButton.gameObject.SetActive(false);
        }
    }
    public void ShowTowerSelectPanel()
    {
        towerSelectPanel.SetActive(true);
    }

    void Update()
    {
        if (DayNightManager.Instance.CurrentPhase != TimePhase.Day)
            return;

        if (isPlacing)
        {
            HandlePlacing();
        }
        else
        {
            // 설치 구역 클릭
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                wp.z = 0;
                var hit = Physics2D.Raycast(wp, Vector2.zero, 0f, placementLayer);
                if (hit.collider != null)
                {
                    selectedArea = hit.collider.transform;
                    towerSelectPanel.transform.position = selectedArea.position;
                    towerSelectPanel.SetActive(true);
                }
                else
                {
                    towerSelectPanel.SetActive(false);
                }
            }
        }
    }

    private void HandlePlacing()
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        wp.z = 0;
        currentIcon.transform.position = wp;

        bool canPlace = placeableArea == null || placeableArea.OverlapPoint(wp);
        iconRenderer.color = canPlace ? new Color(0, 1, 0, 0.6f) : new Color(1, 0, 0, 0.6f);

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (canPlace)
            {
                if (ResourceManager.Instance.SpendGold(selectedCost))
                    PlaceTower(wp);
                else
                    StartCoroutine(FlashInsufficient());
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape)) CancelPlacing();
    }

    private void OnTowerButton(int index)
    {
        // 1) 인덱스 유효성 검사
        if (index < 0 || index >= towerPrefabs.Length)
            return;

        // 2) 골드 지불 시도
        int cost = towerCosts[index];
        if (!ResourceManager.Instance.SpendGold(cost))
        {
            StartCoroutine(FlashInsufficient());
            return;
        }

        // 3) 타워 즉시 생성
        Vector3 spawnPos = selectedArea != null
            ? selectedArea.position
            :new Vector3 (0f, 2.3f, 0f); // selectedArea가 없으면 (0,0,0) 에 생성
        Instantiate(towerPrefabs[index], spawnPos, Quaternion.identity);

        // 4) 설치 이펙트
        if (placeEffectPrefab != null)
            Destroy(Instantiate(placeEffectPrefab, spawnPos, Quaternion.identity), 2f);

        // 5) UI 닫기
        towerSelectPanel.SetActive(false);

        // 6) 이 인덱스 버튼은 더 이상 없앴으니, 비활성화 혹은 비interactable 처리
        //towerSelectButtons[index].gameObject.SetActive(false);

    }


    private void PlaceTower(Vector3 pos)
    {
        Instantiate(selectedPrefab, pos, Quaternion.identity);
        if (placeEffectPrefab != null)
            Destroy(Instantiate(placeEffectPrefab, pos, Quaternion.identity), 2f);
        CancelPlacing();
    }

    private void CancelPlacing()
    {
        isPlacing = false;
        if (currentIcon != null) Destroy(currentIcon);
    }

    private IEnumerator DayRoutine()
    {
        dayMessageText.gameObject.SetActive(true);
        dayMessageText.text = $"Day {GameManager.Instance.CurrentDay} 가 종료되었습니다.";
        yield return new WaitForSeconds(5f);
        dayMessageText.gameObject.SetActive(false);

        // 일정 시간 후 밤 전환 팝업 활성화
        yield return new WaitForSeconds(10f);
        nightPopup.SetActive(true);
    }

    private IEnumerator FlashInsufficient()
    {
        insufficientGoldText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        insufficientGoldText.gameObject.SetActive(false);
    }
}