using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FitSpriteToCamera : MonoBehaviour
{
    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();
        var cam = Camera.main;
        if (sr == null || cam == null || !cam.orthographic) return;

        // ��������Ʈ�� ��ü ���� ũ��
        float spriteWorldHeight = sr.bounds.size.y;
        float spriteWorldWidth = sr.bounds.size.x;

        // ī�޶� ����Ʈ�� Ŀ���ϴ� ���� ũ��
        float camWorldHeight = cam.orthographicSize * 2f;
        float camWorldWidth = camWorldHeight * cam.aspect;

        // ī�޶� ��ũ�⿡ ���� ��������Ʈ ������ ����
        Vector3 newScale = transform.localScale;
        newScale.x = newScale.x * (camWorldWidth / spriteWorldWidth);
        newScale.y = newScale.y * (camWorldHeight / spriteWorldHeight);
        transform.localScale = newScale;

        // ���� ī�޶� ��ġ�� (0,0)�̰� ��������Ʈ�� (0,0)�� ������
        // ȭ�� �Ѱ��� �� ä�����ϴ�.
    }
}
