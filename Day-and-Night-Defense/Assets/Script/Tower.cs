using UnityEngine;
using Debug = UnityEngine.Debug;
using Quaternion = UnityEngine.Quaternion;

public class Tower : MonoBehaviour
{
    [Header("기본 설정")]
    public float hp = 50f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject deathParticle;

    [Header("업그레이드 설정")]
    public int level = 1;                     // 현재 레벨 (1~3)
    public int maxLevel = 3;                  // 최대 레벨
    public int[] upgradeCosts = { 10, 20 };   // 1→2:10G, 2→3:20G
    public float[] rangeByLevel = { 3f, 4f, 5f };
    public float[] fireRateByLevel = { 1f, 0.8f, 0.6f };
    public Sprite[] levelSprites;             // 레벨별 스프라이트 (Size = 3)

    private float attackRange;
    private float fireRate;
    private float fireTimer;

    // 추가: 스프라이트 렌더러 캐싱
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // SpriteRenderer 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();
        ApplyStats();  // 초기 stats & sprite 적용
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

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

    public void Upgrade()
    {
        if (level >= maxLevel)
        {
            Debug.Log("최대 레벨입니다.");
            return;
        }

        int cost = upgradeCosts[level - 1];
        if (ResourceManager.Instance.SpendGold(cost))
        {
            level++;
            ApplyStats();
            Debug.Log($"타워 업그레이드! 현재 레벨: {level}");
        }
        else
        {
            Debug.Log("골드가 부족합니다.");
        }
    }

    void ApplyStats()
    {
        int idx = Mathf.Clamp(level - 1, 0, maxLevel - 1);

        // 1) 공격 범위·발사속도 업데이트
        attackRange = rangeByLevel[idx];
        fireRate = fireRateByLevel[idx];

        // 2) 스프라이트 교체
        if (spriteRenderer != null &&
            levelSprites != null &&
            idx < levelSprites.Length &&
            levelSprites[idx] != null)
        {
            spriteRenderer.sprite = levelSprites[idx];
        }
    }
}
