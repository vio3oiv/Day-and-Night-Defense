using UnityEngine;
using UnityEngine.EventSystems;

public class TowerHoverUIHandler2D : MonoBehaviour
{
    [Header("토글할 UI 오브젝트")]
    public GameObject uiObject;

    [Header("호버 영역 오브젝트 (콜라이더 포함)")]
    public GameObject hoverAreaObject;

    [Header("호버 시 변경할 색상")]
    public Color hoverColor = Color.green;

    // 전역: 어떤 타워라도 UI가 활성화되면 true
    private static bool anyUIActive = false;

    SpriteRenderer spriteRenderer;
    Color originalColor;
    Collider2D hoverAreaCollider;
    bool isUIActive = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (uiObject != null)
            uiObject.SetActive(false);

        if (hoverAreaObject != null)
            hoverAreaCollider = hoverAreaObject.GetComponent<Collider2D>();
    }

    void Update()
    {
        // 이미 다른 타워 UI가 켜져 있으면 아무것도 안 함
        if (anyUIActive && !isUIActive)
            return;

        // 마우스 월드 좌표
        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(wp.x, wp.y);

        // 호버 영역 진입 시 UI 활성화 (최초 한 번)
        if (!isUIActive
            && !anyUIActive
            && hoverAreaCollider != null
            && hoverAreaCollider.OverlapPoint(mousePos2D))
        {
            isUIActive = true;
            anyUIActive = true;
            if (spriteRenderer != null) spriteRenderer.color = hoverColor;
            if (uiObject != null) uiObject.SetActive(true);
        }

        // UI 활성 상태에서 클릭 시, 영역 밖 클릭하면 비활성화
        if (isUIActive && Input.GetMouseButtonDown(0))
        {
            // UI 위 클릭은 무시
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // 호버 영역 밖 클릭 시 끄기
            if (hoverAreaCollider == null || !hoverAreaCollider.OverlapPoint(mousePos2D))
            {
                isUIActive = false;
                anyUIActive = false;
                if (spriteRenderer != null) spriteRenderer.color = originalColor;
                if (uiObject != null) uiObject.SetActive(false);
            }
        }
    }
}
