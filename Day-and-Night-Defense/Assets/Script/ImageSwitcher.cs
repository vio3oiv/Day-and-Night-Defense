using UnityEngine;
using UnityEngine.UI;

public class ImageSwitcher : MonoBehaviour
{
    public Image[] images; // 이미지 배열
    private int currentImageIndex = 0; // 현재 이미지의 인덱스

    void Start()
    {
        for (int i = 0; i < images.Length; i++)
            images[i].gameObject.SetActive(false);
        currentImageIndex = 0;
        ShowCurrentImage();
    }

    // 다음 이미지를 표시하는 함수
    public void ShowNextImage()
    {
        // 현재 이미지 끄기
        HideCurrentImage();

        // 인덱스 증가
        currentImageIndex++;

        // 아직 배열 범위 내라면 다음 이미지 보여주기
        if (currentImageIndex < images.Length)
        {
            ShowCurrentImage();
        }
        else
        {
            // 배열 끝까지 모두 보여줬으면 이 스크립트 비활성화
            this.enabled = false;
        }
    }

    // 현재 이미지를 활성화하는 함수
    void ShowCurrentImage()
    {
        images[currentImageIndex].gameObject.SetActive(true);
    }

    // 현재 이미지를 비활성화하는 함수
    public void HideCurrentImage()
    {
        images[currentImageIndex].gameObject.SetActive(false);
    }
}