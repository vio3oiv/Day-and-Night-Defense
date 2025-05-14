using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ClickToShowUI : MonoBehaviour
{
    [Header("활성화/비활성화할 UI Image")]
    public GameObject uiImageObject;

    private RectTransform imageRect;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;

    // 방금 켰을 때 첫 번째 Update의 클릭은 무시하기 위한 플래그
    private bool _skipNextClose = false;

    void Awake()
    {
        if (uiImageObject == null)
        {
            Debug.LogError("[ClickToShowUI] uiImageObject가 할당되지 않았습니다.");
            enabled = false;
            return;
        }

        imageRect = uiImageObject.GetComponent<RectTransform>();
        var canvas = uiImageObject.GetComponentInParent<Canvas>();
        graphicRaycaster = canvas != null ? canvas.GetComponent<GraphicRaycaster>() : null;
        eventSystem = EventSystem.current;

        uiImageObject.SetActive(false);
    }

    void Update()
    {
        // UI가 켜져 있고, 클릭이 들어왔을 때
        if (uiImageObject.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (_skipNextClose)
            {
                // 방금 켰던 클릭이면 무시하고 플래그 끄기
                _skipNextClose = false;
                return;
            }

            // UI 영역 외부 클릭이면 닫기
            if (!IsPointerOverUI())
                uiImageObject.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        Debug.Log($"[ClickUI] {gameObject.name} OnMouseDown()");
        // 월드 오브젝트 클릭으로 UI 열기
        uiImageObject.SetActive(true);
        _skipNextClose = true;
    }

    private bool IsPointerOverUI()
    {
        if (graphicRaycaster == null || eventSystem == null)
            return false;

        var pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == uiImageObject ||
                result.gameObject.transform.IsChildOf(uiImageObject.transform))
                return true;
        }
        return false;
    }
}
