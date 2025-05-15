using System.Collections;
using UnityEngine;

public class SkillAoe : Skill
{
    [Header("Aoe 세팅")]
    public float attackRange = 3f;               // Inspector에서 설정
    public float damage = 5f;
    public GameObject attackEffectPrefab;        // Inspector에 할당

    protected override void Awake()
    {
        base.Awake();

        // 범위 오버레이 설정
        rangeOverlay.radius = attackRange;
        overlayRenderer.transform.localScale = Vector3.one * attackRange * 2f;
    }

    protected override IEnumerator ActivateRoutine()
    {
        // 1) 표시 모드
        isActive = true;
        overlayRenderer.gameObject.SetActive(true);

        Vector2 clickPos = Vector2.zero;
        // 2) 클릭 대기 & 오버레이 따라다니기
        while (!Input.GetMouseButtonDown(0))
        {
            Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mp.z = 0f;
            overlayRenderer.transform.position = mp;
            yield return null;
        }
        clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //clickPos.y = mp.y; clickPos.x = mp.x; // 이미 z=0

        // 3) 대미지 처리
        Collider2D[] hits = Physics2D.OverlapCircleAll(clickPos, attackRange);
        foreach (var c in hits)
            if (c.CompareTag("Monster"))
                c.GetComponent<Monster>()?.TakeDamage(damage);

        // 4) 이펙트
        if (attackEffectPrefab)
            Instantiate(attackEffectPrefab, clickPos, Quaternion.identity);

        // 5) 정리
        overlayRenderer.gameObject.SetActive(false);
        isActive = false;
        timer = cooldown;
    }
}
