using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Tower : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private float hp = 50f;
    [SerializeField] private GameObject deathParticle;

    [Header("공격 설정")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float fireRate = 1f;

    [Header("업그레이드 설정")]
    [SerializeField] private GameObject[] towerPrefabs;
    [SerializeField] private int[] upgradeCosts;

    [Header("매각 설정")]
    [SerializeField] private GameObject goldDropPrefab;
    [Range(0, 100)][SerializeField] private int sellRefundPercent = 50;

    [Header("설치 비용")]
    [SerializeField] private int placementCost = 10;
    public int PlacementCost => placementCost;

    [Header("범위 표시 설정")]
    [Tooltip("박스 콜라이더 안에 마우스가 있을 때 circle 범위를 표시할 게임오브젝트")]
    [SerializeField] private GameObject rangeIndicator;

    private int currentLevel = 0;
    private float fireTimer = 0f;
    private readonly List<Transform> targets = new();

    private BoxCollider2D healthCollider;
    private CircleCollider2D rangeCollider;

    void Awake()
    {
        healthCollider = GetComponent<BoxCollider2D>();
        healthCollider.isTrigger = true;

        rangeCollider = GetComponent<CircleCollider2D>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius = attackRange;

        // 범위 표시 오브젝트 초기 설정
        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = Vector3.one * attackRange * 2f;
            rangeIndicator.SetActive(false);
        }
    }

    void Update()
    {
        targets.RemoveAll(t => t == null);

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f && targets.Count > 0)
        {
            fireTimer = fireRate;
            Shoot(targets[0]);
        }

        // 마우스 포지션 감지 후 범위 표시 토글
        if (rangeIndicator != null)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool insideHealth = healthCollider.OverlapPoint(mouseWorldPos);
            rangeIndicator.SetActive(insideHealth);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Monster")) return;

        if (rangeCollider.IsTouching(other))
        {
            if (!targets.Contains(other.transform))
                targets.Add(other.transform);
        }

        if (healthCollider.IsTouching(other))
        {
            TakeDamage(10f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Monster") && rangeCollider.IsTouching(other))
        {
            targets.Remove(other.transform);
        }
    }

    private void Shoot(Transform target)
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

    public void Upgrade()
    {
        if (currentLevel >= towerPrefabs.Length - 1)
        {
            Debug.Log("[Upgrade] 이미 최대 레벨입니다.");
            return;
        }

        int cost = upgradeCosts[currentLevel];
        var rm = ResourceManager.Instance;
        if (rm == null || !rm.SpendGold(cost))
        {
            Debug.Log("[Upgrade] 골드 부족 또는 ResourceManager 없음");
            return;
        }

        int nextLevel = ++currentLevel;
        var newTower = Instantiate(towerPrefabs[nextLevel], transform.position, Quaternion.identity)
            .GetComponent<Tower>();
        newTower.currentLevel = nextLevel;
        CopyDataTo(newTower);
        Destroy(gameObject);
    }

    private void CopyDataTo(Tower t)
    {
        t.hp = hp;
        t.deathParticle = deathParticle;
        t.bulletPrefab = bulletPrefab;
        t.firePoint = firePoint;
        t.attackRange = attackRange;
        t.fireRate = fireRate;
        t.towerPrefabs = towerPrefabs;
        t.upgradeCosts = upgradeCosts;
        t.goldDropPrefab = goldDropPrefab;
        t.sellRefundPercent = sellRefundPercent;
        t.placementCost = placementCost;
    }

    public void Sell()
    {
        // 1) 투입된 총비용 계산: 설치비 + 지금까지 쓴 업그레이드비
        int totalInvested = placementCost;
        for (int i = 0; i < currentLevel; i++)
            totalInvested += upgradeCosts[i];

        // 2) 환불 비율 적용
        int refund = Mathf.RoundToInt(totalInvested * (sellRefundPercent / 100f));

        // 3) ResourceManager에 한 번만 추가
        var rm = ResourceManager.Instance;
        if (rm != null)
            rm.AddGold(refund);
        else
            Debug.LogWarning("[Sell] ResourceManager 인스턴스가 없습니다.");

        // 4) 시각 이펙트용 드랍 (수집금액 0으로 설정)
        if (goldDropPrefab != null)
        {
            var go = Instantiate(
                goldDropPrefab,
                transform.position + Vector3.up * 0.5f,
                Quaternion.identity
            );
            var goldScript = go.GetComponent<Gold>();
            if (goldScript != null)
                goldScript.goldAmount = 0;
        }

        // 5) 타워 파괴
        Destroy(gameObject);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        var bc = GetComponent<BoxCollider2D>();
        if (bc != null)
            Gizmos.DrawWireCube(bc.bounds.center, bc.bounds.size);
    }
}
