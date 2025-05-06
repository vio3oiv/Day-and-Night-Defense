using UnityEngine;

public class Gold : MonoBehaviour
{
    public int goldAmount = 10; // 기본 드랍 골드량
    public float lifetime = 2f; // 골드 아이콘 유지 시간

    void Start()
    {
        Invoke(nameof(CollectGold), lifetime);
    }

    void CollectGold()
    {
        // ResourceManager로 골드 획득
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddGold(goldAmount);
        }
        // (선택) 기존 GameManager UI 업데이트가 필요하면 여기도 호출
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddGold(goldAmount);
        }

        Destroy(gameObject);
    }
}
