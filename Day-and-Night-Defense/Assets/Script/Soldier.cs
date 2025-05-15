using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Soldier : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Attack range for engaging monsters")] public float attackRange = 2f;
    [Tooltip("Time between attacks")] public float attackCooldown = 1.5f;
    [Tooltip("Damage dealt per attack")] public float attackPower = 10f;
    [Tooltip("Damage received on contact")] public int contactDamage = 20;
    [Tooltip("Maximum health")] public int maxHealth = 100;

    [Header("Range Indicator")]
    [Tooltip("GameObject (e.g. semi-transparent circle) used to visualize attack range")]
    [SerializeField] private GameObject rangeIndicator;

    private int currentHealth;
    private float attackTimer;
    private Animator anim;
    private bool isDead = false;

    // Attack range collider
    private CircleCollider2D rangeCollider;
    // Contact damage collider
    private BoxCollider2D contactCollider;

    // Targets within attack range
    private readonly List<Transform> attackTargets = new();
    // Monsters frozen on contact
    private readonly List<Monster> frozenMonsters = new();

    void Awake()
    {
        anim = GetComponent<Animator>();

        rangeCollider = GetComponent<CircleCollider2D>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius = attackRange;

        contactCollider = GetComponent<BoxCollider2D>();
        contactCollider.isTrigger = true;

        // Setup range indicator
        if (rangeIndicator != null)
        {
            // Scale twice the radius
            rangeIndicator.transform.localScale = Vector3.one * attackRange * 2f;
            rangeIndicator.SetActive(false);
        }
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        attackTimer = 0f;
        isDead = false;
        anim.Play("IDLE");
    }

    void Update()
    {
        if (isDead) return;
        attackTimer -= Time.deltaTime;

        // Clean up destroyed targets
        attackTargets.RemoveAll(t => t == null);

        // Attack the first target in range when cooldown elapsed
        if (attackTimer <= 0f && attackTargets.Count > 0)
        {
            attackTimer = attackCooldown;
            PerformAttack(attackTargets[0]);
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Monster") || isDead) return;

        // Contact damage and freeze monster
        if (contactCollider.IsTouching(other))
        {
            TakeDamage(contactDamage);
            var m = other.GetComponent<Monster>();
            if (m != null)
            {
                m.FreezeMovement();
                frozenMonsters.Add(m);
            }
        }

        // Add to attack targets if in range
        if (rangeCollider.IsTouching(other) && !attackTargets.Contains(other.transform))
            attackTargets.Add(other.transform);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Monster")) return;
        // Remove from attack targets when leaving range
        if (!rangeCollider.IsTouching(other))
            attackTargets.Remove(other.transform);
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
        // Unfreeze all frozen monsters
        foreach (var m in frozenMonsters)
            if (m != null)
                m.UnfreezeMovement();
        // Destroy soldier after death animation
        Destroy(gameObject, 1.5f);
    }

    // Highlight range on mouse hover
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

    void OnDrawGizmosSelected()
    {
        // Visualize attack range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        // Visualize contact area
        Gizmos.color = Color.red;
        var bc = GetComponent<BoxCollider2D>();
        if (bc != null)
            Gizmos.DrawWireCube(bc.bounds.center, bc.bounds.size);
    }
}
