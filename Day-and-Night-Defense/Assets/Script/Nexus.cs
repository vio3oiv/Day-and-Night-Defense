using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ؼ���(����) ü���� �����մϴ�. ü���� 0�� �Ǹ� ���ӿ����� ȣ���մϴ�.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Nexus : MonoBehaviour
{
    [Header("�ؼ��� ü��")]
    public float maxHealth = 200f;
    public Slider healthSlider;       // ü�¹� UI

    private float currentHealth;

    void Awake()
    {
        // BoxCollider2D�� Ʈ���ŷ� ����
        var bc = GetComponent<BoxCollider2D>();
        bc.isTrigger = true;
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    /// <summary>
    /// �ؼ����� ���ظ� ���� �� ȣ��
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        // 1) ���� ��ü �Ͻ�����
        Time.timeScale = 0f;

        // 2) ���ӿ��� ȣ��
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();

        Debug.Log("�ؼ��� �ı�: ���ӿ��� (���� �Ͻ�����)");
    }

    // �� ���Ϳ� Ʈ���� �浹 �� ������ ����
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Monster")) return;

        // Monster ��ũ��Ʈ�� public float attackPower ������Ƽ�� �ִٰ� ����
        var m = other.GetComponent<Monster>();
        float dmg = (m != null) ? m.attackPower : 10f;  // ������ �⺻ 10
        TakeDamage(dmg);

        Debug.Log($"�ؼ����� ���Ϳ� �ǰ�: {dmg} ����, ���� ü��={currentHealth}");
    }
}
