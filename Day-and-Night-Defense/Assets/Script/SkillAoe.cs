using UnityEngine;

public class SkillAoe : Skill
{
    [Header("광역 스킬 설정")]
    public float aoeRange = 3f;
    public float aoeDamage = 5f;
    public GameObject attackEffectPrefab;

    protected override float GetRange() => aoeRange;

    protected override void FinishCast(Vector3 castPosition)
    {
        // OverlapCircleAll 로 몬스터만 타격
        var hits = Physics2D.OverlapCircleAll(castPosition, aoeRange);
        foreach (var c in hits)
            if (c.CompareTag("Monster"))
                c.GetComponent<Monster>()?.TakeDamage(aoeDamage);

        // 이펙트
        if (attackEffectPrefab != null)
            Instantiate(attackEffectPrefab, castPosition, Quaternion.identity);
    }
}
