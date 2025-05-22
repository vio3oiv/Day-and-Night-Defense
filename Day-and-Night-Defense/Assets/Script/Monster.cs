using System;
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
    public static event Action OnAnyMonsterKilled;

    [Header("피격 이펙트")]
    public GameObject hitParticlePrefab;

    [Header("몬스터 간 분리 힘")]
    public float separationForce = 5f;

    public void FreezeMovement() { /* 이동 불가 로직 */ }
    public void UnfreezeMovement() { /* 이동 재개 로직 */ }

    private Coroutine hurtCoroutine;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        // Rigidbody2D가 Dynamic이어야 서로 밀치고 겹치지 않습니다
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
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
        if (isDead || isBlocked || movePoints == null || movePoints.Count == 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Transform target = movePoints[currentPointIndex];
        Vector2 dir = ((Vector2)target.position - rb.position).normalized;

        // 살짝 흔들리는 움직임
        float wobble = (Mathf.PerlinNoise(Time.time * 0.5f, transform.position.x) - 0.5f) * 0.3f;
        dir.y += wobble;

        // 물리 엔진에 맡겨 밀치기 효과
        rb.linearVelocity = dir * speed;
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

        // ▶ 피격 파티클 생성
        if (hitParticlePrefab != null)
        {
            var fx = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
            Destroy(fx, 1f);  // 1초 후 파괴
        }

        // Hurt 애니메이션 재생 (1초 동안)
        if (hurtCoroutine != null)
            StopCoroutine(hurtCoroutine);
        hurtCoroutine = StartCoroutine(PlayHurtAnimation());

        if (currentHealth <= 0f)
            Die();
    }

    private IEnumerator PlayHurtAnimation()
    {
        anim.Play("Hurt");
        yield return new WaitForSeconds(1f);
        if (!isDead)
            anim.Play("Idle");
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

        if (healthSlider != null)
            sliderRect = healthSlider.GetComponent<RectTransform>();
    }

    void Die()
    {
        isDead = true;
        anim.Play("Dead");
        healthSlider?.gameObject.SetActive(false);
        OnAnyMonsterKilled?.Invoke();

        if (goldPrefab != null)
            Instantiate(goldPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject, 1.5f);
    }

    void OnReachedEnd()
    {
        Destroy(gameObject);
    }

    // ───────────────────────────────────────────────────────
    // 병사와 닿으면 멈추는 기능 모두 주석 처리됨
    // ───────────────────────────────────────────────────────

    /*
    // Trigger 콜라이더용: Is Trigger 켜진 병사와 충돌 시 실행
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Soldier"))
        {
            isBlocked = true;
            Debug.Log("병사와 접촉: 이동 멈춤");
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Soldier"))
        {
            isBlocked = false;
            Debug.Log("병사에서 이탈: 이동 재개");
        }
    }

    // 일반 콜라이더용: Is Trigger 해제된 병사와 충돌 시 실행
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
    */

    void OnCollisionStay2D(Collision2D col)
    {
        // 같은 태그의 몬스터끼리만 처리
        if (col.gameObject.CompareTag("Monster"))
        {
            // 두 몬스터 간의 방향 벡터를 계산
            Vector2 pushDir = (rb.position - (Vector2)col.transform.position).normalized;
            // 약간의 힘을 가해서 밀어냄
            rb.AddForce(pushDir * separationForce, ForceMode2D.Force);
        }
    }
}
