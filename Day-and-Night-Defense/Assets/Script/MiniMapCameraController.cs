using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MiniMapCameraController : MonoBehaviour
{
    [Header("추적 대상")]
    public Transform target;          // TODO: 에디터에서 플레이어 Transform 할당
    [Header("높이 오프셋")]
    public float height = 20f;        // Y축 높이 (2D라면 Z축)
    [Header("부드러운 추적")]
    public float followSmooth = 5f;

    Camera miniCam;

    void Awake()
    {
        miniCam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;
        // 2D 프로젝트라면 주로 Z축을 높이로 사용
        Vector3 desired = new Vector3(
            target.position.x,
            target.position.y + height,
            transform.position.z
        );
        transform.position = Vector3.Lerp(
            transform.position,
            desired,
            followSmooth * Time.deltaTime
        );
    }
}
