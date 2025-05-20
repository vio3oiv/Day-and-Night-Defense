using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[RequireComponent(typeof(LineRenderer))]
public abstract class Skill : MonoBehaviour
{
    public static bool IsCastingSkill { get; private set; }
    [Header("UI")]
    public Button skillButton;
    public Image cooldownFill;
    public TMP_Text cooldownText;

    [Header("스킬 공통")]
    public float cooldown = 5f;
    public int heartCost = 50;

    [Header("오버레이 해상도")]
    [Tooltip("원형을 몇 개의 세그먼트로 그릴지")]
    public int circleSegments = 64;

    protected float timer = 0f;
    protected bool isCasting = false;
    protected LineRenderer line;

    protected virtual void Awake()
    {
        // 1) LineRenderer 세팅
        line = GetComponent<LineRenderer>();
        line.loop = true;
        line.useWorldSpace = true;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.enabled = false;
    }

    protected virtual void Start()
    {
        skillButton.onClick.AddListener(BeginCast);
        UpdateCooldownUI();
    }

    protected virtual void Update()
    {
        // 쿨타이머 감소
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            UpdateCooldownUI();
        }

        if (!isCasting) return;

        // 오버레이 위치를 마우스 월드 좌표로 이동
        Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mp.z = 0f;
        DrawCircle(mp, GetRange());

        // 클릭 시 스킬 발사
        if (Input.GetMouseButtonDown(0) &&
            !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            FinishCast(mp);
            line.enabled = false;
            isCasting = false;
            timer = cooldown;
            UpdateCooldownUI();
        }
        if (Input.GetMouseButtonDown(0) &&
           !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            FinishCast(mp);

            // 시전 종료
            IsCastingSkill = false; // ← 시전 끝
            line.enabled = false;
            isCasting = false;
            timer = cooldown;
            UpdateCooldownUI();
        }
    }

    private void UpdateCooldownUI()
    {
        bool onCooldown = timer > 0f;

        // 쿨타임 중일 때만 보이게
        if (cooldownFill != null) cooldownFill.enabled = onCooldown;
        if (cooldownText != null) cooldownText.enabled = onCooldown;

        if (onCooldown)
        {
            // fillAmount를 timer/cooldown으로 계산 (1→0)
            float pct = Mathf.Clamp01(timer / cooldown);
            cooldownFill.fillAmount = pct;
            cooldownText.text = Mathf.CeilToInt(timer).ToString();
        }
    }



    public void BeginCast()
    {
        // 밤/쿨타임/하트 체크
        if (DayNightManager.Instance.CurrentPhase != TimePhase.Night)
        {
            Debug.Log("스킬은 밤에만 사용 가능합니다.");
            return;
        }
        if (timer > 0f)
        {
            Debug.Log("아직 쿨타임 중입니다.");
            return;
        }
        if (!HeartManager.Instance.Spend(heartCost))
        {
            Debug.Log("하트가 부족합니다.");
            return;
        }

        // 준비 완료
        IsCastingSkill = true;    // ← 시전 시작
        isCasting = true;
        line.enabled = true;
    }

    /// <summary>
    /// 원을 그린다: center 위치, radius 반경
    /// </summary>
    private void DrawCircle(Vector3 center, float radius)
    {
        line.positionCount = circleSegments + 1;
        for (int i = 0; i <= circleSegments; i++)
        {
            float ang = (float)i / circleSegments * Mathf.PI * 2f;
            Vector3 p = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang), 0f) * radius + center;
            line.SetPosition(i, p);
        }
    }

    /// <summary>BeginCast() 에서 radius 로 쓸 값</summary>
    protected abstract float GetRange();
    /// <summary>영역 클릭 시 실행할 실제 스킬 로직</summary>
    protected abstract void FinishCast(Vector3 castPosition);
}
