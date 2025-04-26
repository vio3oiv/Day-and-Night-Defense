using UnityEngine;

public class Gold : MonoBehaviour
{
    public int goldAmount = 10; // 기본 드랍 골드량
    public float lifetime = 2f; // 골드 아이콘 유지 시간

    void Start()
    {
        // 몇 초 후 골드 자동 획득 처리
        Invoke(nameof(CollectGold), lifetime);
    }

    void CollectGold()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddGold(goldAmount);
        }
        Destroy(gameObject); // 골드 아이콘 제거
    }
}
