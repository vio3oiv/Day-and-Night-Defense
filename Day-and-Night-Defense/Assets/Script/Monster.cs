using System.Collections;
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

    [Header("스폰 설정")]
    public float spawnDelayMin = 1f;
    public float spawnDelayMax = 3f;

    [Header("UI")]
    public Slider healthSlider;
    public Vector3 healthBarOffset = new Vector3(0, 0.8f, 0);  // 오프셋 (화면상 머리 위)

    [Header("컴포넌트")]
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [Header("골드 드랍")]
    public GameObject goldPrefab; // 드랍할 골드 프리팹 연결

    private Vector3 moveDirection = Vector3.left;
    private bool isMoving = false;


    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        attackTimer = 0f;
        isDead = false;
        isMoving = false;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        spriteRenderer.flipX = true;
        anim.Play("Idle");

        float delay = Random.Range(spawnDelayMin, spawnDelayMax);
        Invoke(nameof(StartMoving), delay);
    }

    void StartMoving()
    {
        isMoving = true;
        anim.Play("Walk");
    }

    void FixedUpdate()
    {
        if (!isMoving || isDead) return;

        Vector2 move = (Vector2)moveDirection;

        // Y 방향으로 살짝 랜덤 이동 추가
        float randomY = Mathf.PerlinNoise(Time.time * 0.5f, transform.position.x) - 0.5f; // -0.5 ~ +0.5 범위
        move.y += randomY * 0.5f; // Y축 이동량 조정 (부드럽게 흔들림)

        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }


    void Update()
    {
        if (!isMoving || isDead) return;

        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        // 화면 위치로 슬라이더 위치를 맞추기
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

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        anim.Play("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool CanAttack()
    {
        return attackTimer <= 0f;
    }

    public void Attack()
    {
        if (isDead) return;

        attackTimer = attackCooldown;
        anim.Play("Attack");
    }

    void Die()
    {
        isDead = true;
        isMoving = false;
        anim.Play("Dead");

        // 🎯 골드 드랍
        if (goldPrefab != null)
        {
            Instantiate(goldPrefab, transform.position, Quaternion.identity);
        }

        if (healthSlider != null)
            Destroy(healthSlider.gameObject);

        StartCoroutine(DisableAfterDelay(1.5f));
    }

    IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
