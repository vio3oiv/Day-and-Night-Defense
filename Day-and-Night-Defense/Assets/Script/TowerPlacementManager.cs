/// TowerPlacementManager.cs
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 낮 페이즈에 타워 선택 및 설치만 담당합니다.
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
    public GameObject towerSelectPanel;
    public Button[] towerSelectButtons;
    [Tooltip("타워 선택 패널을 여는 버튼")]
    public Button openSelectButton;

    [Header("플레이스 아이콘 & 이펙트")]
    public GameObject towerIconPrefab;
    public GameObject placeEffectPrefab;
    [Header("스폰 포인트")]
    [Tooltip("인스펙터에서 할당할 스폰 위치들 (Transform)")]
    public Transform[] spawnPoints;

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

        // 오픈 선택 버튼 설정
        if (openSelectButton != null)
        {
            openSelectButton.onClick.AddListener(() =>
            {
                if (selectedArea != null)
                    towerSelectPanel.transform.position = selectedArea.position;
                towerSelectPanel.SetActive(true);
            });
        }
    }

    void Update()
    {
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
        if (index < 0 || index >= towerPrefabs.Length) return;

        int cost = towerCosts[index];
        if (!ResourceManager.Instance.SpendGold(cost))
        {
            StartCoroutine(FlashInsufficient());
            return;
        }
        // 모든 포인트에 타워 설치
        foreach (Transform sp in spawnPoints)
        {
            Instantiate(towerPrefabs[index], sp.position, Quaternion.identity);
            if (placeEffectPrefab != null)
                Destroy(Instantiate(placeEffectPrefab, sp.position, Quaternion.identity), 2f);
        }

        /*Vector3 spawnPos = selectedArea != null
            ? selectedArea.position
            : Vector3.zero;
        Instantiate(towerPrefabs[index], spawnPos, Quaternion.identity);
        if (placeEffectPrefab != null)
            Destroy(Instantiate(placeEffectPrefab, spawnPos, Quaternion.identity), 2f);*/

        towerSelectPanel.SetActive(false);
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

    private IEnumerator FlashInsufficient()
    {
        
        yield break;
    }
}

