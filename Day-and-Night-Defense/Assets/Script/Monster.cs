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
    private bool isBlocked = false;  // 타워 충돌로 이동 차단 플래그

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
    public Canvas uiCanvas;                // 캔버스 할당
    private RectTransform canvasRect;      // 캔버스 RectTransform
    private Camera uiCamera;              // World→Screen 변환용 카메라
    private RectTransform sliderRect;

    [Header("골드 드랍")]
    public GameObject goldPrefab;

    public void FreezeMovement() { /* 이동 불가 로직 */ }
    public void UnfreezeMovement() { /* 이동 재개 로직 */ }

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Canvas 세팅
        canvasRect = uiCanvas.GetComponent<RectTransform>();
        uiCamera = (uiCanvas.renderMode == RenderMode.ScreenSpaceCamera)
                     ? uiCanvas.worldCamera
                     : null;

        // Slider RectTransform 캐싱
        if (healthSlider != null)
            sliderRect = healthSlider.GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        attackTimer = 0f;
        isDead = false;
        isBlocked = false;
        currentPointIndex = 0;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        anim.Play("Idle");
    }

    void FixedUpdate()
    {
        // 죽었거나 타워에 막혀 있으면 이동 중단
        if (isDead || isBlocked)
            return;

        if (movePoints == null || movePoints.Count == 0)
            return;

        Transform target = movePoints[currentPointIndex];
        Vector2 dir = ((Vector2)target.position - rb.position).normalized;

        // 살짝 흔들리는 움직임 추가 (선택)
        float wobble = (Mathf.PerlinNoise(Time.time * 0.5f, transform.position.x) - 0.5f) * 0.3f;
        dir.y += wobble;

        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
        spriteRenderer.flipX = dir.x < 0f;

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
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
        if (healthSlider == null || sliderRect == null) return;

        // 체력값 갱신
        healthSlider.value = currentHealth;

        // 월드 → 캔버스 로컬좌표
        Vector3 worldPos = transform.position + healthBarOffset;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            uiCamera,
            out Vector2 localPos
        );

        // 앵커드포지션 적용
        sliderRect.anchoredPosition = localPos;
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
        // TODO: 실제 데미지 로직 삽입
        if (healthSlider != null)
            sliderRect = healthSlider.GetComponent<RectTransform>();
    }

    void Die()
    {
        isDead = true;
        anim.Play("Dead");
        healthSlider?.gameObject.SetActive(false);

        if (goldPrefab != null)
            Instantiate(goldPrefab, transform.position, Quaternion.identity);

        //GameManager.Instance?.OnMonsterKilled();
        StartCoroutine(DisableAfterDelay(1.5f));
    }

    IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    void OnReachedEnd()
    {
        //GameManager.Instance?.OnMonsterKilled();
        gameObject.SetActive(false);
    }

    // Trigger 콜라이더용: Is Trigger 켜진 타워와 충돌 시 실행
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Soldier"))
        {
            isBlocked = true;
            Debug.Log("타워와 접촉: 이동 멈춤");
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Soldier"))
        {
            isBlocked = false;
            Debug.Log("타워에서 이탈: 이동 재개");
        }
    }

    // 일반 콜라이더용: Is Trigger 해제된 타워와 충돌 시 실행
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.isTrigger && col.gameObject.CompareTag("Soldier"))
            isBlocked = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (!col.collider.isTrigger && col.gameObject.CompareTag("Soldier"))
            isBlocked = false;
    }
}