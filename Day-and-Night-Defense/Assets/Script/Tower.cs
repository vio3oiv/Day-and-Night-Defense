using UnityEngine;
using System.Diagnostics;
using System.Numerics;
using System.Resources;
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

    private float attackRange;
    private float fireRate;
    private float fireTimer;

    void Start()
    {
        ApplyStats();  // 현재 레벨에 맞춰 사거리·발사속도 초기화
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        // 사거리 내 몬스터 탐지
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
    /// 타워 업그레이드 시도 (외부에서 호출)
    /// </summary>
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
            // TODO: 레벨별 스프라이트 or 이펙트 교체
            Debug.Log($"타워 업그레이드! 현재 레벨: {level}");
        }
        else
        {
            Debug.Log("골드가 부족합니다.");
        }
    }

    /// <summary>
    /// level에 맞춰 attackRange, fireRate 적용
    /// </summary>
    void ApplyStats()
    {
        int idx = Mathf.Clamp(level - 1, 0, maxLevel - 1);
        attackRange = rangeByLevel[idx];
        fireRate = fireRateByLevel[idx];
    }
}
