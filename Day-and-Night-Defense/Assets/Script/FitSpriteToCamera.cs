using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FitSpriteToCamera : MonoBehaviour
{
    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();
        var cam = Camera.main;
        if (sr == null || cam == null || !cam.orthographic) return;

        // 스프라이트의 전체 월드 크기
        float spriteWorldHeight = sr.bounds.size.y;
        float spriteWorldWidth = sr.bounds.size.x;

        // 카메라 뷰포트가 커버하는 월드 크기
        float camWorldHeight = cam.orthographicSize * 2f;
        float camWorldWidth = camWorldHeight * cam.aspect;

        // 카메라 뷰크기에 맞춰 스프라이트 스케일 조정
        Vector3 newScale = transform.localScale;
        newScale.x = newScale.x * (camWorldWidth / spriteWorldWidth);
        newScale.y = newScale.y * (camWorldHeight / spriteWorldHeight);
        transform.localScale = newScale;

        // 이제 카메라 위치가 (0,0)이고 스프라이트가 (0,0)에 있으면
        // 화면 한가득 꽉 채워집니다.
    }
}
