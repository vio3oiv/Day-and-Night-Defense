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

    [Header("��ų ����")]
    public float cooldown = 5f;
    public int heartCost = 50;

    [Header("�������� �ػ�")]
    [Tooltip("������ �� ���� ���׸�Ʈ�� �׸���")]
    public int circleSegments = 64;

    protected float timer = 0f;
    protected bool isCasting = false;
    protected LineRenderer line;

    protected virtual void Awake()
    {
        // 1) LineRenderer ����
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
        // ��Ÿ�̸� ����
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            UpdateCooldownUI();
        }

        if (!isCasting) return;

        // �������� ��ġ�� ���콺 ���� ��ǥ�� �̵�
        Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mp.z = 0f;
        DrawCircle(mp, GetRange());

        // Ŭ�� �� ��ų �߻�
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

            // ���� ����
            IsCastingSkill = false; // �� ���� ��
            line.enabled = false;
            isCasting = false;
            timer = cooldown;
            UpdateCooldownUI();
        }
    }

    private void UpdateCooldownUI()
    {
        bool onCooldown = timer > 0f;

        // ��Ÿ�� ���� ���� ���̰�
        if (cooldownFill != null) cooldownFill.enabled = onCooldown;
        if (cooldownText != null) cooldownText.enabled = onCooldown;

        if (onCooldown)
        {
            // fillAmount�� timer/cooldown���� ��� (1��0)
            float pct = Mathf.Clamp01(timer / cooldown);
            cooldownFill.fillAmount = pct;
            cooldownText.text = Mathf.CeilToInt(timer).ToString();
        }
    }



    public void BeginCast()
    {
        // ��/��Ÿ��/��Ʈ üũ
        if (DayNightManager.Instance.CurrentPhase != TimePhase.Night)
        {
            Debug.Log("��ų�� �㿡�� ��� �����մϴ�.");
            return;
        }
        if (timer > 0f)
        {
            Debug.Log("���� ��Ÿ�� ���Դϴ�.");
            return;
        }
        if (!HeartManager.Instance.Spend(heartCost))
        {
            Debug.Log("��Ʈ�� �����մϴ�.");
            return;
        }

        // �غ� �Ϸ�
        IsCastingSkill = true;    // �� ���� ����
        isCasting = true;
        line.enabled = true;
    }

    /// <summary>
    /// ���� �׸���: center ��ġ, radius �ݰ�
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

    /// <summary>BeginCast() ���� radius �� �� ��</summary>
    protected abstract float GetRange();
    /// <summary>���� Ŭ�� �� ������ ���� ��ų ����</summary>
    protected abstract void FinishCast(Vector3 castPosition);
}
