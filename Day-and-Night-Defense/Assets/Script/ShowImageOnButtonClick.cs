﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Assign this to a GameObject in the scene to show or hide a UI Image when Buttons are clicked,
/// and also open it by pressing Escape—unless a tower/soldier placement or skill cast is active.
/// </summary>
public class ShowImageOnButtonClick : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Button that shows the image")]
    public Button showButton;
    [Tooltip("Button that hides the image")]
    public Button closeButton;
    [Tooltip("Image to show or hide when buttons are clicked")]
    public Image targetImage;

    [Header("Settings")]
    [Tooltip("Should the show button toggle visibility on each click?")]
    public bool toggleOnClick = false;

    private void Awake()
    {
        // Ensure image starts hidden
        if (targetImage != null)
            targetImage.gameObject.SetActive(false);

        // Register show button listener
        if (showButton != null)
            showButton.onClick.AddListener(OnShowButtonClicked);
        else
            Debug.LogWarning("[ShowImageOnButtonClick] showButton is not assigned.");

        // Register close button listener
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        else
            Debug.LogWarning("[ShowImageOnButtonClick] closeButton is not assigned.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && targetImage != null)
        {
            // 배치/스킬 중일 때는 무시
            if (TowerDragAndDrop.Instance?.IsPlacing == true) return;
            if (SoldierDragAndDrop.Instance?.IsPlacing == true) return;
            if (Skill.IsCastingSkill) return;

            // 열려 있지 않으면 열고, 열려 있으면 닫기
            if (!targetImage.gameObject.activeSelf)
                targetImage.gameObject.SetActive(true);
            else
                targetImage.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Unregister listeners to prevent memory leaks
        if (showButton != null)
            showButton.onClick.RemoveListener(OnShowButtonClicked);
        if (closeButton != null)
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
    }

    private void OnShowButtonClicked()
    {
        if (targetImage == null) return;

        if (toggleOnClick)
        {
            bool isActive = targetImage.gameObject.activeSelf;
            targetImage.gameObject.SetActive(!isActive);
        }
        else
        {
            targetImage.gameObject.SetActive(true);
        }
    }

    private void OnCloseButtonClicked()
    {
        if (targetImage == null) return;
        targetImage.gameObject.SetActive(false);
    }
}
