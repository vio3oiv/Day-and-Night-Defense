using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Soldier : MonoBehaviour
{
    [Header("Stats")]
    public float attackCooldown = 1.5f;
    public float attackPower = 10f;
    public int contactDamage = 20;
    public int maxHealth = 100;
    [SerializeField] private GameObject deathParticle;

    [Header("Range Indicator")]
    [Tooltip("Semi-transparent circle to show attack range")]
    [SerializeField] private GameObject rangeIndicator;

    [Header("Health Bar")]
    public Slider healthSlider;
    public Vector3 healthBarOffset = new Vector3(0f, 1.0f, 0f);
    public Canvas uiCanvas;

    private int currentHealth;
    private float attackTimer;
    private Animator anim;
    private bool isDead = false;
    private bool isBlocked = false;

    private CircleCollider2D rangeCollider;
    private BoxCollider2D contactCollider;

    private readonly List<Transform> attackTargets = new();
    private readonly List<Monster> frozenMonsters = new();

    // UI positioning
    private RectTransform canvasRect;
    private Camera uiCamera;
    private RectTransform sliderRect;


    void Awake()
    {
        anim = GetComponent<Animator>();

        rangeCollider = GetComponent<CircleCollider2D>();
        rangeCollider.isTrigger = true;

        contactCollider = GetComponent<BoxCollider2D>();
        contactCollider.isTrigger = false;
        // Setup range indicator
        if (rangeIndicator != null)
        {
            float radius = rangeCollider.radius;
            var sr = rangeIndicator.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                float originalDiameter = sr.sprite.bounds.size.x;
                float desiredDiameter = radius * 2f;
                float scale = desiredDiameter / originalDiameter;
                rangeIndicator.transform.localScale = new Vector3(scale, scale, 1f);
            }
            else
            {
                rangeIndicator.transform.localScale = Vector3.one * (radius * 2f);
            }
            rangeIndicator.transform.localPosition = rangeCollider.offset;
            rangeIndicator.SetActive(false);
        }

        // UI Canvas setup
        if (uiCanvas != null)
        {
            canvasRect = uiCanvas.GetComponent<RectTransform>();
            uiCamera = uiCanvas.renderMode == RenderMode.ScreenSpaceCamera
                       ? uiCanvas.worldCamera
                       : null;
        }

        // Health slider setup
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            sliderRect = healthSlider.GetComponent<RectTransform>();
        }
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        attackTimer = 0f;
        isDead = false;
        anim.Play("IDLE");
        if (healthSlider != null)
            healthSlider.gameObject.SetActive(true);
    }

    void Update()
    {
        if (isDead) return;
        if (isDead || isBlocked) return;

        // Attack logic
        attackTimer -= Time.deltaTime;
        attackTargets.RemoveAll(t => t == null);
        if (attackTimer <= 0f && attackTargets.Count > 0)
        {
            attackTimer = attackCooldown;
            PerformAttack(attackTargets[0]);
        }

        // Update health bar position
        if (healthSlider != null && sliderRect != null && canvasRect != null)
        {
            healthSlider.value = currentHealth;
            Vector3 worldPos = transform.position + healthBarOffset;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPoint,
                uiCamera,
                out Vector2 localPos
            );
            sliderRect.anchoredPosition = localPos;
        }
    }

    private void PerformAttack(Transform target)
    {
        if (isDead) return;
        anim.Play("ATTACK");
        var m = target.GetComponent<Monster>();
        if (m != null)
            m.TakeDamage(attackPower);
    }

    void OnMouseEnter()
    {
        if (rangeIndicator != null && !isDead)
            rangeIndicator.SetActive(true);
    }

    void OnMouseExit()
    {
        if (rangeIndicator != null)
            rangeIndicator.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Monster") || isDead) return;

        // ⇨ 이 부분은 범위 감지만
        if (rangeCollider.IsTouching(other)
            && !attackTargets.Contains(other.transform))
        {
            attackTargets.Add(other.transform);
            Debug.Log($"[{name}] Detected monster in range: {other.name}");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Monster")) return;

        if (contactCollider.IsTouching(other) == false)
        {
            isBlocked = false;
            Debug.Log($"Soldier unblocked by {other.name}");
        }

        if (!rangeCollider.IsTouching(other))
        {
            attackTargets.Remove(other.transform);
            Debug.Log($"[{name}] Monster left range: {other.name}");
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Monster") && !isDead)
        {
            isBlocked = true;
            TakeDamage(contactDamage);
            Debug.Log($"Soldier took damage: {contactDamage} from {col.collider.name}");

            var m = col.collider.GetComponent<Monster>();
            if (m != null)
            {
                m.FreezeMovement();
                frozenMonsters.Add(m);
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag("Monster"))
        {
            isBlocked = false;
            Debug.Log($"Soldier unblocked by {col.collider.name}");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        anim.Play("DAMAGED");
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        anim.Play("DEATH");
        // 1) 언프리즈
        foreach (var m in frozenMonsters)
            if (m != null) m.UnfreezeMovement();

        // 2) 이펙트
        if (deathParticle != null)
            Instantiate(deathParticle, transform.position, Quaternion.identity);
        if (healthSlider != null)
            healthSlider.gameObject.SetActive(false);

        // 3) 삭제 (필요시 지연)
        Destroy(gameObject, 1.5f);

    }

    void OnDrawGizmosSelected()
    {
        if (rangeCollider != null)
        {
            Vector3 worldCenter = transform.TransformPoint(rangeCollider.offset);
            float worldRadius = rangeCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(worldCenter, worldRadius);
        }
        var bc = GetComponent<BoxCollider2D>();
        if (bc != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(bc.bounds.center, bc.bounds.size);
        }
    }
} //