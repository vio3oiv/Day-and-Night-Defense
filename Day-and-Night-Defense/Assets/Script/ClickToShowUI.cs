using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ClickToShowUI : MonoBehaviour
{
    [Header("Ȱ��ȭ/��Ȱ��ȭ�� UI Image")]
    public GameObject uiImageObject;

    private RectTransform imageRect;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;

    // ��� ���� �� ù ��° Update�� Ŭ���� �����ϱ� ���� �÷���
    private bool _skipNextClose = false;

    // **ù Ŭ�� ���ÿ� �÷���**
    private bool _hasIgnoredFirstClick = false;

    void Awake()
    {
        if (uiImageObject == null)
        {
            Debug.LogError("[ClickToShowUI] uiImageObject�� �Ҵ���� �ʾҽ��ϴ�.");
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
        if (!uiImageObject.activeSelf)
            return;

        // ESC Ű�� �ݱ�
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiImageObject.SetActive(false);
            return;
        }

        // ���콺 Ŭ�� �˻�
        if (Input.GetMouseButtonDown(0))
        {
            if (_skipNextClose)
            {
                // ��� �״� Ŭ���̸� �����ϰ� �÷��� ����
                _skipNextClose = false;
                return;
            }

            // UI ���� �ܺ� Ŭ���̸� �ݱ�
            if (!IsPointerOverUI())
                uiImageObject.SetActive(false);
        }
    }
    void OnMouseDown()
    {
        // ù Ŭ���� ���ø� �ϰ� �ƹ� ���۵� ���� ����
        if (!_hasIgnoredFirstClick)
        {
            _hasIgnoredFirstClick = true;
            return;
        }

        // ���� ��ġ ��� ���̸� UI ���� ����
        if (SoldierDragAndDrop.Instance != null && SoldierDragAndDrop.Instance.IsPlacing)
            return;

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
