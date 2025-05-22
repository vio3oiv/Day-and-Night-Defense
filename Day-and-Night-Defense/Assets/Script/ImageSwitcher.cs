using UnityEngine;
using UnityEngine.UI;

public class ImageSwitcher : MonoBehaviour
{
    public Image[] images; // �̹��� �迭
    private int currentImageIndex = 0; // ���� �̹����� �ε���

    void Start()
    {
        // ��� �̹��� ��Ȱ��ȭ �� ù ��° �̹��� Ȱ��ȭ
        for (int i = 0; i < images.Length; i++)
            images[i].gameObject.SetActive(false);
        currentImageIndex = 0;
        ShowCurrentImage();
    }

    // ���� �̹����� ǥ���ϴ� �Լ�
    public void ShowNextImage()
    {
        HideCurrentImage();
        currentImageIndex++;
        if (currentImageIndex < images.Length)
        {
            ShowCurrentImage();
        }
        else
        {
            // �迭 ���� �����ϸ� ��ũ��Ʈ ��Ȱ��ȭ
            this.enabled = false;
        }
    }

    // ���� �̹����� ǥ���ϴ� �Լ�
    public void ShowPreviousImage()
    {
        HideCurrentImage();
        currentImageIndex--;
        if (currentImageIndex >= 0)
        {
            ShowCurrentImage();
        }
        else
        {
            // ù �̹��� �������� �Ѿ�� �ʵ��� 0���� ����
            currentImageIndex = 0;
            ShowCurrentImage();
        }
    }

    // ���� �̹����� Ȱ��ȭ�ϴ� �Լ�
    private void ShowCurrentImage()
    {
        images[currentImageIndex].gameObject.SetActive(true);
    }

    // ���� �̹����� ��Ȱ��ȭ�ϴ� �Լ�
    private void HideCurrentImage()
    {
        images[currentImageIndex].gameObject.SetActive(false);
    }
}
