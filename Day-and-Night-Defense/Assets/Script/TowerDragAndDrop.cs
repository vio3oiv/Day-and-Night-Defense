using UnityEngine;
using UnityEngine.EventSystems;

// This makes “Tower Drag and Drop” appear under a custom menu in Add Component
[AddComponentMenu("Custom/Tower Drag and Drop")]
public class TowerDragAndDrop : MonoBehaviour
{
    [Header("Tower Prefab Settings")]
    public GameObject towerPrefab;
    public GameObject towerIconPrefab;
    public GameObject placeEffectPrefab; // 🎇 placement effect prefab

    [Header("Placement Area")]
    public Collider2D placeableArea;

    private GameObject currentIcon;
    private SpriteRenderer iconRenderer;
    private bool isPlacing = false;

    void Update()
    {
        if (!isPlacing || currentIcon == null)
            return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        currentIcon.transform.position = worldPos;

        if (placeableArea != null)
            UpdateIconColor(placeableArea.OverlapPoint(worldPos));

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            TryPlaceTower(worldPos);

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelPlacing();
    }

    public void StartPlacingTower()
    {
        if (isPlacing)
        {
            CancelPlacing();
            return;
        }

        isPlacing = true;
        currentIcon = Instantiate(towerIconPrefab);
        iconRenderer = currentIcon.GetComponent<SpriteRenderer>();
        if (iconRenderer == null)
            Debug.LogWarning("TowerIconPrefab is missing a SpriteRenderer!");
    }

    private void TryPlaceTower(Vector3 position)
    {
        if (placeableArea.OverlapPoint(position))
        {
            Instantiate(towerPrefab, position, Quaternion.identity);

            if (placeEffectPrefab != null)
            {
                var fx = Instantiate(placeEffectPrefab, position, Quaternion.identity);
                Destroy(fx, 2f);
            }

            EndPlacing();
        }
        else
        {
            Debug.Log("Cannot place tower here.");
        }
    }

    private void EndPlacing()
    {
        if (currentIcon != null)
            Destroy(currentIcon);
        isPlacing = false;
    }

    private void CancelPlacing()
    {
        Debug.Log("Placement cancelled.");
        EndPlacing();
    }

    private void UpdateIconColor(bool canPlace)
    {
        if (iconRenderer == null) return;
        iconRenderer.color = canPlace
            ? new Color(0f, 1f, 0f, 0.6f)  // green
            : new Color(1f, 0f, 0f, 0.6f); // red
    }
}
