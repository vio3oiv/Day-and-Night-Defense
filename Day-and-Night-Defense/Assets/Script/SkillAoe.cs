using System.Collections;
using UnityEngine;

public class SkillAoe : Skill
{
    [Header("Aoe ����")]
    public float attackRange = 3f;               // Inspector���� ����
    public float damage = 5f;
    public GameObject attackEffectPrefab;        // Inspector�� �Ҵ�

    protected override void Awake()
    {
        base.Awake();

        // ���� �������� ����
        rangeOverlay.radius = attackRange;
        overlayRenderer.transform.localScale = Vector3.one * attackRange * 2f;
    }

    protected override IEnumerator ActivateRoutine()
    {
        // 1) ǥ�� ���
        isActive = true;
        overlayRenderer.gameObject.SetActive(true);

        Vector2 clickPos = Vector2.zero;
        // 2) Ŭ�� ��� & �������� ����ٴϱ�
        while (!Input.GetMouseButtonDown(0))
        {
            Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mp.z = 0f;
            overlayRenderer.transform.position = mp;
            yield return null;
        }
        clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //clickPos.y = mp.y; clickPos.x = mp.x; // �̹� z=0

        // 3) ����� ó��
        Collider2D[] hits = Physics2D.OverlapCircleAll(clickPos, attackRange);
        foreach (var c in hits)
            if (c.CompareTag("Monster"))
                c.GetComponent<Monster>()?.TakeDamage(damage);

        // 4) ����Ʈ
        if (attackEffectPrefab)
            Instantiate(attackEffectPrefab, clickPos, Quaternion.identity);

        // 5) ����
        overlayRenderer.gameObject.SetActive(false);
        isActive = false;
        timer = cooldown;
    }
}
