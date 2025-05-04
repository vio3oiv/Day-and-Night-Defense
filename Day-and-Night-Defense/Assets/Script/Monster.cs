using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    [Header("스탯")]
    public float speed = 1.5f;
    public float maxHealth = 100f;
    public float attackPower = 10f;
    public float attackCooldown = 1.5f;

    private float currentHealth;
    private float attackTimer;
    private bool isDead = false;

    [Header("경로 이동")]
    [Tooltip("SpawnManager에서 할당해 주는 경로 포인트 리스트")]
    [HideInInspector]
    public List<Transform> movePoints = new List<Transform>();
    private int currentPointIndex = 0;

    [Header("UI")]
    public Slider healthSlider;
    public Vector3 healthBarOffset = new Vector3(0, 0.8f, 0);

    [Header("컴포넌트")]
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [Header("골드 드랍")]
    public GameObject goldPrefab;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        // 초기화
        currentHealth = maxHealth;
        attackTimer = 0f;
        isDead = false;
        currentPointIndex = 0;

        // UI 세팅
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        anim.Play("Idle");
    }

    void FixedUpdate()
    {
        if (isDead) return;
        // 경로가 없으면 이동 안 함
        if (movePoints == null || movePoints.Count == 0) return;

        Transform target = movePoints[currentPointIndex];
        Vector2 dir = ((Vector2)target.position - rb.position).normalized;

        // 살짝 흔들리는 움직임 추가 (선택)
        float wobble = (Mathf.PerlinNoise(Time.time * 0.5f, transform.position.x) - 0.5f) * 0.3f;
        dir.y += wobble;

        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
        spriteRenderer.flipX = dir.x < 0f;

        // 포인트에 도달했으면 다음으로
        if (Vector2.Distance(rb.position, target.position) < 0.1f)
        {
            currentPointIndex++;
            if (currentPointIndex >= movePoints.Count)
                OnReachedEnd();
        }
    }

    void Update()
    {
        if (isDead) return;

        // 공격 쿨다운
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        // 슬라이더 위치 갱신
        if (healthSlider != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + healthBarOffset);
            healthSlider.transform.position = screenPos;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        healthSlider?.SetValueWithoutNotify(currentHealth);
        anim.Play("Hurt");

        if (currentHealth <= 0f)
            Die();
    }

    public bool CanAttack()
    {
        return attackTimer <= 0f && !isDead;
    }

    public void Attack()
    {
        if (!CanAttack()) return;

        attackTimer = attackCooldown;
        anim.Play("Attack");
        // TODO: 실제 데미지 로직 삽입 (예: 충돌박스, 레이 등)
    }

    void Die()
    {
        isDead = true;
        anim.Play("Dead");
        healthSlider?.gameObject.SetActive(false);

        // 골드 드랍
        if (goldPrefab != null)
            Instantiate(goldPrefab, transform.position, Quaternion.identity);

        StartCoroutine(DisableAfterDelay(1.5f));
    }

    IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    void OnReachedEnd()
    {
        // 경로 끝에 도달했을 때 처리 (예: 기지 공격, 파괴 등)
        // 지금은 그냥 비활성화
        gameObject.SetActive(false);
    }
}
