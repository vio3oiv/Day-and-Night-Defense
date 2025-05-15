using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  

public abstract class Skill : MonoBehaviour
{
    [Header("쿨타임 UI")]
    public Image cooldownFill;          // Inspector에 할당
    public TMP_Text cooldownText;       // 혹은 public Text cooldownText;

    [Header("공통")]
    public float cooldown = 5f;
    public int heartCost = 50;
    [HideInInspector] public bool IsReady => timer <= 0f;
    protected float timer = 0f;

    protected CircleCollider2D rangeOverlay;
    protected SpriteRenderer overlayRenderer;
    protected bool isActive = false;

    protected virtual void Awake()
    {
        // UI 초기화
        if (cooldownFill) cooldownFill.fillAmount = 0f;
        if (cooldownText) cooldownText.text = "";

        // 범위 표시용 콜라이더 & 오버레이
        rangeOverlay = gameObject.AddComponent<CircleCollider2D>();
        rangeOverlay.isTrigger = true;
        overlayRenderer = new GameObject("RangeOverlay").AddComponent<SpriteRenderer>();
        overlayRenderer.transform.SetParent(transform, false);
        overlayRenderer.color = new Color(1, 0, 0, 0.3f);
        overlayRenderer.gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        // 쿨타이머 감소 및 UI 갱신
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            float pct = Mathf.Clamp01(timer / cooldown);
            if (cooldownFill) cooldownFill.fillAmount = 1f - pct;
            if (cooldownText) cooldownText.text = Mathf.CeilToInt(timer).ToString();
        }
    }

    public void TryActivate()
    {
        if (DayNightManager.Instance.CurrentPhase != TimePhase.Night) { Debug.Log("밤에만 사용 가능"); return; }
        if (!IsReady) { Debug.Log("아직 쿨타임"); return; }
        if (!HeartManager.Instance.Spend(heartCost)) { Debug.Log("하트 부족"); return; }
        StartCoroutine(ActivateRoutine());
    }

    protected abstract IEnumerator ActivateRoutine();
}
