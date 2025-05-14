using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("기본 설정")]
    public float hp = 50f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject deathParticle;

    [Header("공격 설정")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float fireRate = 1f;
    private float fireTimer = 0f;

    [Header("레벨별 타워 프리팹 (0=레벨1, 1=레벨2, …)")]
    public GameObject[] towerPrefabs;

    [Header("레벨별 업그레이드 비용")]
    public int[] upgradeCosts;

    [Header("팔 때 드랍할 골드 오브젝트(prefab)")]
    public GameObject goldDropPrefab;

    [Header("매각 환불 비율(%)")]
    [Range(0, 100)]
    public int sellRefundPercent = 50;

    [Header("설치 비용")]
    [Tooltip("이 타워를 설치할 때 필요한 골드")]
    public int placementCost = 10;

    // 현재 레벨 (0부터 시작)
    private int currentLevel = 0;

    void Update()
    {
        fireTimer -= Time.deltaTime;

        // 범위 내 몬스터 탐지
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Monster") && fireTimer <= 0f)
            {
                fireTimer = fireRate;
                Shoot(hit.transform);
                break;
            }
        }
    }

    void Shoot(Transform target)
    {
        var bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity)
                         .GetComponent<Bullet>();
        bullet.Init(target);
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0f)
        {
            Instantiate(deathParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
            TakeDamage(10f);
    }

    /// <summary>
    /// 타워를 한 단계 업그레이드합니다.
    /// ResourceManager에서 골드를 차감하고,
    /// 새로운 레벨 프리팹을 생성한 뒤 자신을 파괴합니다.
    /// 최대 레벨이면 호버 UI를 비활성화합니다.
    /// </summary>
    public void Upgrade()
    {
        // 1) 최대 레벨 체크
        if (currentLevel >= towerPrefabs.Length - 1)
        {
            Debug.Log("[Upgrade] 이미 최대 레벨입니다.");
            return;
        }

        int cost = upgradeCosts[currentLevel];
        var rm = ResourceManager.Instance;
        if (rm == null)
        {
            Debug.LogError("[Upgrade] ResourceManager 인스턴스 없음");
            return;
        }

        // 2) 골드 부족 체크
        if (rm.Gold < cost)
        {
            Debug.Log($"[Upgrade] 골드 부족: 필요 {cost}, 보유 {rm.Gold}");
            return;
        }

        // 3) 골드 차감
        rm.SpendGold(cost);
        Debug.Log($"[Upgrade] 레벨{currentLevel + 1} → 레벨{currentLevel + 2} (비용 {cost})");

        // 4) 다음 레벨 프리팹 생성
        int nextLevel = currentLevel + 1;
        GameObject newTowerGO = Instantiate(
            towerPrefabs[nextLevel],
            transform.position,
            Quaternion.identity
        );
        Tower newTower = newTowerGO.GetComponent<Tower>();

        // 5) 값 복사
        newTower.currentLevel = nextLevel;
        newTower.bulletPrefab = this.bulletPrefab;
        newTower.firePoint = this.firePoint;
        newTower.deathParticle = this.deathParticle;
        newTower.attackRange = this.attackRange;
        newTower.fireRate = this.fireRate;
        newTower.goldDropPrefab = this.goldDropPrefab;
        newTower.sellRefundPercent = this.sellRefundPercent;

        // 6) 최대 레벨 타워라면 호버 UI 비활성화
        if (nextLevel >= towerPrefabs.Length - 1)
        {
            var hover = newTower.GetComponent<TowerHoverUIHandler2D>();
            if (hover != null)
            {
                hover.enabled = false;
                if (hover.uiObject != null)
                    hover.uiObject.SetActive(false);
            }
        }

        // 7) 기존 타워 파괴
        Destroy(gameObject);
    }

    /// <summary>
    /// 타워를 팔아서 일정 비율만큼 골드를 환불하고, 골드 드랍 오브젝트를 생성합니다.
    /// </summary>
    public void Sell()
    {
        int baseValue = upgradeCosts[currentLevel];
        int refund = Mathf.RoundToInt(baseValue * (sellRefundPercent / 100f));

        Debug.Log($"[Sell] 레벨{currentLevel + 1} 타워 매각: 환불 {refund}골드");

        if (goldDropPrefab != null)
        {
            var go = Instantiate(
                goldDropPrefab,
                transform.position + Vector3.up * 0.5f,
                Quaternion.identity
            );
            var goldScript = go.GetComponent<Gold>();
            if (goldScript != null)
                goldScript.goldAmount = refund;
        }
        else
        {
            Debug.LogWarning("[Sell] goldDropPrefab이 할당되어 있지 않습니다.");
        }

        Destroy(gameObject);
    }
}
