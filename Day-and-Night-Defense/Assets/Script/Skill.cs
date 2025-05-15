using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  

public abstract class Skill : MonoBehaviour
{
    [Header("��Ÿ�� UI")]
    public Image cooldownFill;          // Inspector�� �Ҵ�
    public TMP_Text cooldownText;       // Ȥ�� public Text cooldownText;

    [Header("����")]
    public float cooldown = 5f;
    public int heartCost = 50;
    [HideInInspector] public bool IsReady => timer <= 0f;
    protected float timer = 0f;

    protected CircleCollider2D rangeOverlay;
    protected SpriteRenderer overlayRenderer;
    protected bool isActive = false;

    protected virtual void Awake()
    {
        // UI �ʱ�ȭ
        if (cooldownFill) cooldownFill.fillAmount = 0f;
        if (cooldownText) cooldownText.text = "";

        // ���� ǥ�ÿ� �ݶ��̴� & ��������
        rangeOverlay = gameObject.AddComponent<CircleCollider2D>();
        rangeOverlay.isTrigger = true;
        overlayRenderer = new GameObject("RangeOverlay").AddComponent<SpriteRenderer>();
        overlayRenderer.transform.SetParent(transform, false);
        overlayRenderer.color = new Color(1, 0, 0, 0.3f);
        overlayRenderer.gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        // ��Ÿ�̸� ���� �� UI ����
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
        if (DayNightManager.Instance.CurrentPhase != TimePhase.Night) { Debug.Log("�㿡�� ��� ����"); return; }
        if (!IsReady) { Debug.Log("���� ��Ÿ��"); return; }
        if (!HeartManager.Instance.Spend(heartCost)) { Debug.Log("��Ʈ ����"); return; }
        StartCoroutine(ActivateRoutine());
    }

    protected abstract IEnumerator ActivateRoutine();
}
