using UnityEngine;
using UnityEngine.UI;

public class ImageSwitcher : MonoBehaviour
{
    public Image[] images; // �̹��� �迭
    private int currentImageIndex = 0; // ���� �̹����� �ε���

    void Start()
    {
        for (int i = 0; i < images.Length; i++)
            images[i].gameObject.SetActive(false);
        currentImageIndex = 0;
        ShowCurrentImage();
    }

    // ���� �̹����� ǥ���ϴ� �Լ�
    public void ShowNextImage()
    {
        // ���� �̹��� ����
        HideCurrentImage();

        // �ε��� ����
        currentImageIndex++;

        // ���� �迭 ���� ����� ���� �̹��� �����ֱ�
        if (currentImageIndex < images.Length)
        {
            ShowCurrentImage();
        }
        else
        {
            // �迭 ������ ��� ���������� �� ��ũ��Ʈ ��Ȱ��ȭ
            this.enabled = false;
        }
    }

    // ���� �̹����� Ȱ��ȭ�ϴ� �Լ�
    void ShowCurrentImage()
    {
        images[currentImageIndex].gameObject.SetActive(true);
    }

    // ���� �̹����� ��Ȱ��ȭ�ϴ� �Լ�
    public void HideCurrentImage()
    {
        images[currentImageIndex].gameObject.SetActive(false);
    }
}