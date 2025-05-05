using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 넥서스(기지) 체력을 관리합니다. 체력이 0이 되면 게임오버를 호출합니다.
/// </summary>
public class Nexus : MonoBehaviour
{
    [Header("넥서스 체력")]
    public float maxHealth = 200f;
    public Slider healthSlider;       // 체력바 UI

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
    /// 넥서스가 피해를 받을 때 호출
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
        // 게임오버 호출
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();

        // 넥서스 파괴 애니메이션·이펙트 추가 가능
        Debug.Log("넥서스 파괴: 게임오버");
    }
}