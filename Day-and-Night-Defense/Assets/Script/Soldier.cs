using UnityEngine;

public class Soldier : MonoBehaviour
{
    [Header("스탯")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float attackPower = 10f;
    public int maxHealth = 100;

    private int currentHealth;
    private float attackTimer;
    private Animator anim;
    private bool isDead = false;

    private Transform target;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        attackTimer = 0f;
        anim.Play("IDLE");
    }

    void Update()
    {
        if (isDead) return;

        attackTimer -= Time.deltaTime;

        if (target == null || Vector2.Distance(transform.position, target.position) > attackRange)
        {
            FindTarget();
        }

        if (target != null)
        {
            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }
    }

    void FindTarget()
    {
        float minDist = float.MaxValue;
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");

        foreach (var m in monsters)
        {
            float dist = Vector2.Distance(transform.position, m.transform.position);
            if (dist < attackRange && dist < minDist)
            {
                minDist = dist;
                target = m.transform;
            }
        }
    }

    void Attack()
    {
        if (target == null) return;

        anim.Play("ATTACK");

        Monster m = target.GetComponent<Monster>();
        if (m != null)
        {
            m.TakeDamage(attackPower);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        anim.Play("DAMAGED");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.Play("DEATH");
        Destroy(gameObject, 1.5f);
    }
}
