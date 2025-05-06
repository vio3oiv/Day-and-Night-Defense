using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraState { Locked, SemiLocked, Unlocked }

    [Header("���� ���")]
    public CameraState state = CameraState.SemiLocked;

    [Header("����(Chase) ����")]
    // public Transform target;               // TODO: �÷��̾� Transform �� �Ҵ��ϼ���
    public Vector3 followOffset = new Vector3(0f, 0f, -10f);
    [Range(0f, 20f)] public float followSmooth = 5f;

    [Header("���� �д�(Pan) ����")]
    public float panSpeed = 20f;
    public float edgePanThickness = 10f;   // ȭ�� �����ڸ� �д� ���� �β� (px)
    private bool userPanning = false;

    [Header("��(Zoom) ����")]
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

    // ���콺 �ٷ� �� ���� ����
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

    // ȭ�� �����ڸ���Ű����� �д�
    void HandlePan()
    {
        Vector3 pan = Vector3.zero;
        Vector2 mouse = Input.mousePosition;

        // Ű����
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) pan.x -= panSpeed;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) pan.x += panSpeed;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) pan.y += panSpeed;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) pan.y -= panSpeed;

        // ȭ�� ����
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
            // �д� ���߸� �ٽ� ���� ����
            userPanning = false;
        }
    }

    // ���� �Ǵ� ����
    void HandleFollow()
    {
        if (state == CameraState.Locked || (state == CameraState.SemiLocked && !userPanning))
        {
            // TODO: ���߿� target �� �Ҵ��ϸ� �Ʒ� �ּ� �����ϰ� ����ϼ���.
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

    // �����̽��ٷ� ��� �߾� ����
    void CenterOnTarget()
    {
        // TODO: target �Ҵ� �� ����
        // if (Input.GetKeyDown(KeyCode.Space) && target != null)
        // {
        //     transform.position = target.position + followOffset;
        // }
    }

    // V Ű ���� ��� ��ȯ (Locked �� SemiLocked �� Unlocked �� ��)
    void HandleModeSwitch()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            state = (CameraState)(((int)state + 1) % System.Enum.GetNames(typeof(CameraState)).Length);
            userPanning = false;  // ��� ��ȯ �� ���� �д� ���� �ʱ�ȭ
            Debug.Log("Camera Mode: " + state);
        }
    }
}
