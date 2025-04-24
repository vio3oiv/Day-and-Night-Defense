using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;        // �ӵ��� ������ ����
    public float damage = 10f;
    private Transform target;

    // Ÿ������ ������ Ÿ���� �޾� �ʱ�ȭ
    public void Init(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);  // Ÿ���� ������ٸ� �ҷ��� ����
            return;
        }

        // Ÿ�� �������� �̵�
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

            Destroy(gameObject);  // �浹 �� �ı�
        }
    }
}
