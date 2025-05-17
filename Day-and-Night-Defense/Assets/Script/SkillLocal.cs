using UnityEngine;

public class SkillLocal : Skill
{
    [Header("국소 스킬 설정")]
    public float localRange = 2f;
    public float localDamage = 20f;
    public GameObject attackEffectPrefab;

    protected override float GetRange() => localRange;

    protected override void FinishCast(Vector3 castPosition)
    {
        // 클릭 지점 중심 small range
        var hits = Physics2D.OverlapCircleAll(castPosition, localRange);
        foreach (var c in hits)
            if (c.CompareTag("Monster"))
                c.GetComponent<Monster>()?.TakeDamage(localDamage);

        if (attackEffectPrefab != null)
            Instantiate(attackEffectPrefab, castPosition, Quaternion.identity);
    }
}
