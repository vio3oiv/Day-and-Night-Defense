using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 넥서스(기지) 체력을 관리합니다. 체력이 0이 되면 게임오버를 호출합니다.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Nexus : MonoBehaviour
{
    [Header("넥서스 체력")]
    public float maxHealth = 200f;
    public Slider healthSlider;       // 체력바 UI

    private float currentHealth;

    void Awake()
    {
        // BoxCollider2D를 트리거로 설정
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
        // 1) 게임 전체 일시정지
        Time.timeScale = 0f;

        // 2) 게임오버 호출
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();

        Debug.Log("넥서스 파괴: 게임오버 (게임 일시정지)");
    }

    // ↓ 몬스터와 트리거 충돌 시 데미지 적용
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Monster")) return;

        // Monster 스크립트에 public float attackPower 프로퍼티가 있다고 가정
        var m = other.GetComponent<Monster>();
        float dmg = (m != null) ? m.attackPower : 10f;  // 없으면 기본 10
        TakeDamage(dmg);

        Debug.Log($"넥서스가 몬스터에 피격: {dmg} 피해, 남은 체력={currentHealth}");
    }
}
