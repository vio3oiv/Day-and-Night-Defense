using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class TowerHoverUIHandler2D : MonoBehaviour
{
    [Header("토글할 UI 오브젝트")]
    public GameObject uiObject;

    [Header("호버 영역 오브젝트 (콜라이더 포함)")]
    public GameObject hoverAreaObject;

    [Header("호버 시 변경할 색상")]
    public Color hoverColor = Color.green;

    [Header("UI 클릭 검사용 Graphic Raycaster")]
    public GraphicRaycaster uiRaycaster;  // 반드시 인스펙터에 할당!

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Collider2D hoverAreaCollider;

    // UI 클릭 판별용
    private PointerEventData pointerData;
    private List<RaycastResult> raycastResults = new List<RaycastResult>();

    // 다른 타워 UI 동시 활성화 방지
    private static bool anyUIActive = false;
    private bool isUIActive = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (uiObject != null)
            uiObject.SetActive(false);

        if (hoverAreaObject != null)
            hoverAreaCollider = hoverAreaObject.GetComponent<Collider2D>();

        pointerData = new PointerEventData(EventSystem.current);
    }

    void Update()
    {
        // 다른 타워 UI가 켜져 있고, 자신은 꺼져 있으면 동작 중지
        if (anyUIActive && !isUIActive)
            return;

        // 마우스 위치 → 월드 좌표
        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(wp.x, wp.y);

        // 1) 호버 영역 진입 시 UI 활성화
        if (!isUIActive && hoverAreaCollider != null && hoverAreaCollider.OverlapPoint(mousePos2D))
        {
            ActivateUI();
        }

        // 2) UI 활성 상태에서 클릭 감지 → 닫기 판정
        if (isUIActive && Input.GetMouseButtonDown(0))
        {
            // (a) 호버 영역 안 클릭 여부
            bool clickedOnHover = hoverAreaCollider != null && hoverAreaCollider.OverlapPoint(mousePos2D);

            // (b) UI 위 클릭 여부 (Raycast)
            bool clickedOnUI = false;
            if (uiRaycaster != null)
            {
                pointerData.position = Input.mousePosition;
                raycastResults.Clear();
                uiRaycaster.Raycast(pointerData, raycastResults);

                clickedOnUI = raycastResults.Any(r =>
                    r.gameObject.transform.IsChildOf(uiObject.transform)
                );
            }
            else
            {
                // uiRaycaster 가 할당되지 않았을 경우 로그
                Debug.LogWarning("[TowerHoverUI] uiRaycaster가 할당되지 않아 UI 클릭 검사를 건너뜁니다.");
            }

            // 둘 다 아니면 닫기
            if (!clickedOnHover && !clickedOnUI)
                CloseUI();
        }
    }

    private void ActivateUI()
    {
        isUIActive = true;
        anyUIActive = true;
        if (spriteRenderer != null)
            spriteRenderer.color = hoverColor;
        if (uiObject != null)
            uiObject.SetActive(true);
    }

    private void CloseUI()
    {
        isUIActive = false;
        anyUIActive = false;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
        if (uiObject != null)
            uiObject.SetActive(false);
    }

    void OnDisable()
    {
        if (isUIActive) CloseUI();
    }

    void OnDestroy()
    {
        if (isUIActive) CloseUI();
    }
}
