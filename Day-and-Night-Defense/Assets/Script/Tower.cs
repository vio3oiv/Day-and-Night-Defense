using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("�⺻ ����")]
    public float hp = 50f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject deathParticle;

    [Header("���׷��̵� ����")]
    public int level = 1;                     // ���� ���� (1~3)
    public int maxLevel = 3;                  // �ִ� ����
    public int[] upgradeCosts = { 10, 20 };   // 1��2:10G, 2��3:20G
    public float[] rangeByLevel = { 3f, 4f, 5f };
    public float[] fireRateByLevel = { 1f, 0.8f, 0.6f };

    private float attackRange;
    private float fireRate;
    private float fireTimer;

    void Start()
    {
        ApplyStats();  // ���� ������ ���� ��Ÿ����߻�ӵ� �ʱ�ȭ
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        // ��Ÿ� �� ���� Ž��
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
    /// Ÿ�� ���׷��̵� �õ� (�ܺο��� ȣ��)
    /// </summary>
    public void Upgrade()
    {
        if (level >= maxLevel)
        {
            Debug.Log("�ִ� �����Դϴ�.");
            return;
        }

        int cost = upgradeCosts[level - 1];
        if (ResourceManager.Instance.SpendGold(cost))
        {
            level++;
            ApplyStats();
            // TODO: ������ ��������Ʈ or ����Ʈ ��ü
            Debug.Log($"Ÿ�� ���׷��̵�! ���� ����: {level}");
        }
        else
        {
            Debug.Log("��尡 �����մϴ�.");
        }
    }

    /// <summary>
    /// level�� ���� attackRange, fireRate ����
    /// </summary>
    void ApplyStats()
    {
        int idx = Mathf.Clamp(level - 1, 0, maxLevel - 1);
        attackRange = rangeByLevel[idx];
        fireRate = fireRateByLevel[idx];
    }
}
