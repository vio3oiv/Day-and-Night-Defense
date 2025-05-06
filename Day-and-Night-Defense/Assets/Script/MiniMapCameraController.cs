using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MiniMapCameraController : MonoBehaviour
{
    [Header("���� ���")]
    public Transform target;          // TODO: �����Ϳ��� �÷��̾� Transform �Ҵ�
    [Header("���� ������")]
    public float height = 20f;        // Y�� ���� (2D��� Z��)
    [Header("�ε巯�� ����")]
    public float followSmooth = 5f;

    Camera miniCam;

    void Awake()
    {
        miniCam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;
        // 2D ������Ʈ��� �ַ� Z���� ���̷� ���
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
