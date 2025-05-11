using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Gold : MonoBehaviour
{
    [Header("획득 골드량")]
    [SerializeField] public int goldAmount = 10;
    [Header("자동 수집 대기 시간")]
    [SerializeField] private float lifetime = 2f;

    private bool _collected = false;

    void Start()
    {
        // lifetime 뒤에 자동으로 수집 처리
        Invoke(nameof(AutoCollect), lifetime);
    }

    // 마우스 클릭 시 수집 (PC 환경용)
    void OnMouseDown()
    {
        Collect();
    }

    // 트리거 충돌 시 수집 (플레이어 캐릭터에 Collider2D + Tag "Player" 지정)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_collected && other.CompareTag("Player"))
            Collect();
    }

    private void AutoCollect()
    {
        if (!_collected)
            Collect();
    }

    private void Collect()
    {
        _collected = true;

        // ResourceManager에만 골드 추가
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.AddGold(goldAmount);
        else
            Debug.LogWarning("[Gold] ResourceManager 인스턴스가 없습니다.");

        // 이후 이 오브젝트는 파괴
        Destroy(gameObject);
    }
}
