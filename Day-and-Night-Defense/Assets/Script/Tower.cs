using UnityEngine;

public class Tower : MonoBehaviour
{
    public float hp = 50f;
    public float attackRange = 3f;
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject deathParticle;

    private float fireTimer;

    void Update()
    {
        fireTimer -= Time.deltaTime;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Monster") && fireTimer <= 0)
            {
                fireTimer = fireRate;
                Shoot(hit.transform);
                break;
            }

        }
    }

    void Shoot(Transform target)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(target);
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            Instantiate(deathParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            TakeDamage(10f);
        }
    }
}
