using UnityEngine;
using UnityEngine.UI;

public class ImageSwitcher : MonoBehaviour
{
    public Image[] images; // 이미지 배열
    private int currentImageIndex = 0; // 현재 이미지의 인덱스

    void Start()
    {
        // 모든 이미지 비활성화 후 첫 번째 이미지 활성화
        for (int i = 0; i < images.Length; i++)
            images[i].gameObject.SetActive(false);
        currentImageIndex = 0;
        ShowCurrentImage();
    }

    // 다음 이미지를 표시하는 함수
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
            // 배열 끝에 도달하면 스크립트 비활성화
            this.enabled = false;
        }
    }

    // 이전 이미지를 표시하는 함수
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
            // 첫 이미지 이전으로 넘어가지 않도록 0으로 고정
            currentImageIndex = 0;
            ShowCurrentImage();
        }
    }

    // 현재 이미지를 활성화하는 함수
    private void ShowCurrentImage()
    {
        images[currentImageIndex].gameObject.SetActive(true);
    }

    // 현재 이미지를 비활성화하는 함수
    private void HideCurrentImage()
    {
        images[currentImageIndex].gameObject.SetActive(false);
    }
}
