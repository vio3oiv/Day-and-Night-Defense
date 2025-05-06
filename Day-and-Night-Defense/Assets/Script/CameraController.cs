using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraState { Locked, SemiLocked, Unlocked }

    [Header("현재 모드")]
    public CameraState state = CameraState.SemiLocked;

    [Header("추적(Chase) 설정")]
    // public Transform target;               // TODO: 플레이어 Transform 을 할당하세요
    public Vector3 followOffset = new Vector3(0f, 0f, -10f);
    [Range(0f, 20f)] public float followSmooth = 5f;

    [Header("수동 패닝(Pan) 설정")]
    public float panSpeed = 20f;
    public float edgePanThickness = 10f;   // 화면 가장자리 패닝 영역 두께 (px)
    private bool userPanning = false;

    [Header("줌(Zoom) 설정")]
    public float[] zoomSizes = new float[] { 5f, 7f, 10f };
    public int currentZoom = 1;
    [Range(1f, 20f)] public float zoomSmooth = 10f;

    void Update()
    {
        HandleZoom();
        HandlePan();
        HandleFollow();
        HandleModeSwitch();
    }

    // 마우스 휠로 줌 레벨 변경
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f && currentZoom > 0) currentZoom--;
        if (scroll < 0f && currentZoom < zoomSizes.Length - 1) currentZoom++;

        float targetOrtho = zoomSizes[currentZoom];
        Camera.main.orthographicSize = Mathf.Lerp(
            Camera.main.orthographicSize,
            targetOrtho,
            zoomSmooth * Time.deltaTime
        );
    }

    // 화면 가장자리·키보드로 패닝
    void HandlePan()
    {
        Vector3 pan = Vector3.zero;
        Vector2 mouse = Input.mousePosition;

        // 키보드
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) pan.x -= panSpeed;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) pan.x += panSpeed;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) pan.y += panSpeed;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) pan.y -= panSpeed;

        // 화면 엣지
        if (mouse.x <= edgePanThickness) pan.x -= panSpeed;
        else if (mouse.x >= Screen.width - edgePanThickness) pan.x += panSpeed;
        if (mouse.y <= edgePanThickness) pan.y -= panSpeed;
        else if (mouse.y >= Screen.height - edgePanThickness) pan.y += panSpeed;

        if (pan != Vector3.zero)
        {
            userPanning = true;
            transform.Translate(pan * Time.deltaTime, Space.World);
        }
        else if (userPanning && state == CameraState.SemiLocked)
        {
            // 패닝 멈추면 다시 추적 복귀
            userPanning = false;
        }
    }

    // 추적 또는 정지
    void HandleFollow()
    {
        if (state == CameraState.Locked || (state == CameraState.SemiLocked && !userPanning))
        {
            // TODO: 나중에 target 을 할당하면 아래 주석 해제하고 사용하세요.
            // if (target != null)
            // {
            //     Vector3 desiredPos = target.position + followOffset;
            //     transform.position = Vector3.Lerp(
            //         transform.position,
            //         desiredPos,
            //         followSmooth * Time.deltaTime
            //     );
            // }
        }
    }

    // 스페이스바로 즉시 중앙 복귀
    void CenterOnTarget()
    {
        // TODO: target 할당 후 해제
        // if (Input.GetKeyDown(KeyCode.Space) && target != null)
        // {
        //     transform.position = target.position + followOffset;
        // }
    }

    // V 키 눌러 모드 순환 (Locked → SemiLocked → Unlocked → …)
    void HandleModeSwitch()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            state = (CameraState)(((int)state + 1) % System.Enum.GetNames(typeof(CameraState)).Length);
            userPanning = false;  // 모드 전환 시 기존 패닝 상태 초기화
            Debug.Log("Camera Mode: " + state);
        }
    }
}
