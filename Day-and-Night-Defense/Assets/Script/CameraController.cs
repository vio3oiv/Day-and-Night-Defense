using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraState { Locked, SemiLocked, Unlocked }

    [Header("카메라 모드")]
    public CameraState state = CameraState.SemiLocked;

    [Header("추적(Chase) 설정")]
    // public Transform target;               // TODO: 플레이어 Transform을 연결하세요.
    public Vector3 followOffset = new Vector3(0f, 0f, -10f);
    [Range(0f, 20f)] public float followSmooth = 5f;

    [Header("팬(Pan) 설정")]
    public float panSpeed = 20f;
    public float edgePanThickness = 10f;   // 화면 가장자리에서 팬 시작 두께(px)
    private bool userPanning = false;

    [Header("줌(Zoom) 설정")]
    public float[] zoomSizes = new float[] { 5f, 7f, 10f };
    public int currentZoom = 1;
    [Range(1f, 20f)] public float zoomSmooth = 10f;

    [Header("이동 제한 수치 범위")]
    public float minX = -19.5f, maxX = 19.5f;
    public float minY = -10.5f, maxY = 10.5f;

    // (선택) 콜라이더 방식으로도 제한하고 싶다면 이걸 할당
    public Collider2D confineCollider = null;

    void Update()
    {
        HandleZoom();
        HandlePan();
        HandleFollow();
        HandleModeSwitch();
        ClampPosition();
    }

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

    void HandlePan()
    {
        Vector3 pan = Vector3.zero;
        Vector2 mouse = Input.mousePosition;

        // 키보드 팬
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) pan.x -= panSpeed;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) pan.x += panSpeed;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) pan.y += panSpeed;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) pan.y -= panSpeed;

        // 마우스 엣지 팬
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
            // 한 번 팬이 일어나면 SemiLocked 모드에서는 고정 해제
            userPanning = false;
        }
    }

    void HandleFollow()
    {
        if (state == CameraState.Locked || (state == CameraState.SemiLocked && !userPanning))
        {
            // TODO: 플레이어 target이 연결되면 아래 주석 해제
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

    void HandleModeSwitch()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            state = (CameraState)(((int)state + 1) % System.Enum.GetNames(typeof(CameraState)).Length);
            userPanning = false;
            Debug.Log("Camera Mode: " + state);
        }
    }

    void ClampPosition()
    {
        // 카메라 뷰포트 절반 크기 계산
        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;

        if (confineCollider != null)
        {
            // 콜라이더 안쪽으로만 이동
            Vector2 camPos2D = new Vector2(transform.position.x, transform.position.y);
            Vector2 clamped2D = confineCollider.ClosestPoint(camPos2D);
            transform.position = new Vector3(clamped2D.x, clamped2D.y, transform.position.z);
        }
        else
        {
            // 숫자 범위 방식으로만 제한 (뷰포트 가장자리가 절대 영역 밖으로 나가지 않도록)
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x,
                minX + camHalfWidth,
                maxX - camHalfWidth);
            pos.y = Mathf.Clamp(pos.y,
                minY + camHalfHeight,
                maxY - camHalfHeight);
            transform.position = pos;
        }
    }
}
