using UnityEngine;

public class SkillAoe : Skill
{
    [Header("���� ��ų ����")]
    public float aoeRange = 3f;
    public float aoeDamage = 5f;
    public GameObject attackEffectPrefab;

    protected override float GetRange() => aoeRange;

    protected override void FinishCast(Vector3 castPosition)
    {
        // OverlapCircleAll �� ���͸� Ÿ��
        var hits = Physics2D.OverlapCircleAll(castPosition, aoeRange);
        foreach (var c in hits)
            if (c.CompareTag("Monster"))
                c.GetComponent<Monster>()?.TakeDamage(aoeDamage);

        // ����Ʈ
        if (attackEffectPrefab != null)
            Instantiate(attackEffectPrefab, castPosition, Quaternion.identity);
    }
}
