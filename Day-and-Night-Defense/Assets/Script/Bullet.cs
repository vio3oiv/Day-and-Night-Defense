using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;        // 속도를 빠르게 조정
    public float damage = 10f;
    private Transform target;

    // 타워에서 지정한 타겟을 받아 초기화
    public void Init(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);  // 타겟이 사라졌다면 불렛도 제거
            return;
        }

        // 타겟 방향으로 이동
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
            }

            Destroy(gameObject);  // 충돌 후 파괴
        }
    }
}
