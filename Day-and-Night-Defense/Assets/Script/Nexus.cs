using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ؼ���(����) ü���� �����մϴ�. ü���� 0�� �Ǹ� ���ӿ����� ȣ���մϴ�.
/// </summary>
public class Nexus : MonoBehaviour
{
    [Header("�ؼ��� ü��")]
    public float maxHealth = 200f;
    public Slider healthSlider;       // ü�¹� UI

    private float currentHealth;

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
        // ���ӿ��� ȣ��
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();

        // �ؼ��� �ı� �ִϸ��̼ǡ�����Ʈ �߰� ����
        Debug.Log("�ؼ��� �ı�: ���ӿ���");
    }
}