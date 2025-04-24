using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    [Header("스탯")]
    public float speed = 1.5f;
    public float maxHealth = 100f;
    public float attackPower = 10f;
    public float attackCooldown = 1.5f;

    private float currentHealth;
    private float attackTimer;

    [Header("스폰 설정")]
    public float spawnDelayMin = 1f;
    public float spawnDelayMax = 3f;

    [Header("UI")]
    public Image healthFillImage;      // 게이지 바
    public Canvas healthCanvas;        // 월드 스페이스 캔버스
    public Vector3 healthBarOffset = new Vector3(0, 1f, 0);

    private Vector3 moveDirection = Vector3.left;
    private bool isSpawning = false;

    void OnEnable()
    {
        // 체력 초기화
        currentHealth = maxHealth;
        attackTimer = 0f;

        if (healthFillImage != null)
            healthFillImage.fillAmount = 1f;

        // 체력바가 월드 공간 상단에 위치
        if (healthCanvas != null)
            healthCanvas.worldCamera = Camera.main;

        // 스폰 지연 후 이동 시작
        float delay = Random.Range(spawnDelayMin, spawnDelayMax);
        Invoke(nameof(StartMoving), delay);
    }

    void StartMoving()
    {
        isSpawning = true;
    }

    void Update()
    {
        if (!isSpawning) return;

        // 이동 처리
        transform.position += moveDirection * speed * Time.deltaTime;

        // 체력바 따라가기
        if (healthCanvas != null)
            healthCanvas.transform.position = transform.position + healthBarOffset;

        // 공격 쿨타임 갱신
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (healthFillImage != null)
            healthFillImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool CanAttack()
    {
        return attackTimer <= 0f;
    }

    public void Attack()
    {
        attackTimer = attackCooldown;
        // 여기서 타겟 데미지 처리 가능
    }

    void Die()
    {
        if (healthCanvas != null)
            Destroy(healthCanvas.gameObject);

        gameObject.SetActive(false);
    }
}
