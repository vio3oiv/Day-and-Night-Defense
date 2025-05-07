using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class MiniMapViewportUI : MonoBehaviour
{
    [Header("참조 설정")]
    public Camera mainCamera;            // 실제 게임 플레이 카메라
    public Camera miniMapCamera;         // 위에서 내려다보는 미니맵 카메라

    [Header("UI 참조")]
    public RectTransform miniMapRect;    // RawImage (미니맵) RectTransform
    public RectTransform viewportFrame;  // 붉은 테두리 UI Image의 RectTransform

    void Update()
    {
        if (mainCamera == null || miniMapCamera == null ||
            miniMapRect == null || viewportFrame == null)
            return;

        // 1) 메인 카메라가 보고 있는 월드 좌표 Rect 구하기
        float camHalfH = mainCamera.orthographicSize;
        float camHalfW = camHalfH * mainCamera.aspect;

        // 미니맵 카메라 기준 원점(월드 좌표)
        Vector3 miniOrigin = miniMapCamera.transform.position;

        // 2) 메인 카메라 중심과 미니맵 중심 간 오프셋
        Vector2 worldOffset = new Vector2(
            mainCamera.transform.position.x - miniOrigin.x,
            mainCamera.transform.position.y - miniOrigin.y
        );

        // 3) UI 상 미니맵 크기 (픽셀)
        float uiW = miniMapRect.rect.width;
        float uiH = miniMapRect.rect.height;

        // 4) 월드->UI 스케일 (한쪽 절반 크기 기준)
        float scaleX = uiW / (miniMapCamera.orthographicSize * 2f);
        float scaleY = uiH / (miniMapCamera.orthographicSize * 2f);

        // 5) 뷰포트 크기 픽셀 계산
        Vector2 viewSize = new Vector2(
            camHalfW * 2f * scaleX,
            camHalfH * 2f * scaleY
        );
        viewportFrame.sizeDelta = viewSize;

        // 6) 뷰포트 위치(pixel) 계산
        Vector2 uiOffset = new Vector2(
            worldOffset.x * scaleX,
            worldOffset.y * scaleY
        );
        viewportFrame.anchoredPosition = uiOffset;
    }
}
