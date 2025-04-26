using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPlacementManager : MonoBehaviour
{
    [Header("타워 프리팹 설정")]
    public GameObject towerPrefab;
    public GameObject towerIconPrefab;
    public GameObject placeEffectPrefab; // 🎇 설치 이펙트 프리팹

    [Header("설치 가능 범위 설정")]
    public Collider2D placeableArea;

    private GameObject currentIcon;
    private SpriteRenderer iconRenderer;
    private bool isPlacing = false;

    void Update()
    {
        if (!isPlacing || currentIcon == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        currentIcon.transform.position = mouseWorldPos;

        if (placeableArea != null)
        {
            bool canPlace = placeableArea.OverlapPoint(mouseWorldPos);
            UpdateIconColor(canPlace);
        }

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            TryPlaceTower(mouseWorldPos);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacing();
        }
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
        {
            Debug.LogWarning("TowerIconPrefab에 SpriteRenderer가 없습니다!");
        }
    }

    void TryPlaceTower(Vector3 position)
    {
        if (placeableArea.OverlapPoint(position))
        {
            Instantiate(towerPrefab, position, Quaternion.identity);

            // 🎇 설치 이펙트 재생
            if (placeEffectPrefab != null)
            {
                GameObject effect = Instantiate(placeEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 2f); // 2초 후 자동 삭제
            }

            EndPlacing();
        }
        else
        {
            Debug.Log("설치 불가 지역입니다.");
        }
    }

    void EndPlacing()
    {
        if (currentIcon != null)
        {
            Destroy(currentIcon);
        }
        isPlacing = false;
    }

    void CancelPlacing()
    {
        Debug.Log("설치 취소");
        EndPlacing();
    }

    void UpdateIconColor(bool canPlace)
    {
        if (iconRenderer == null) return;

        if (canPlace)
        {
            iconRenderer.color = new Color(0f, 1f, 0f, 0.6f);
        }
        else
        {
            iconRenderer.color = new Color(1f, 0f, 0f, 0.6f);
        }
    }
}
