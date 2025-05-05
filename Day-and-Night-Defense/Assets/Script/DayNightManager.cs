using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TimePhase { Day, Night }

/// <summary>
/// ��/�� ��ȯ�� �����մϴ�. T Ű�� 5�ʰ� ���� ����� ��ȯ, ���̺� ���� �� �ڵ� ������ ��ȯ
/// UI �����̴�, �ð��� �ؽ�Ʈ, ������ ǥ��, ȭ�� ��Ӱ� ó�� ����
/// </summary>
public class DayNightManager : MonoBehaviour
{
    public static DayNightManager Instance { get; private set; }

    [Header("�ð��� ����")]
    public TimePhase CurrentPhase = TimePhase.Day;
    public float holdDuration = 5f;      // T Ű ���� �ð�

    [Header("UI ����")]
    public Slider holdSlider;            // T ���� ���൵ ǥ��
    public TextMeshProUGUI phaseText;               // "DAY" / "NIGHT" �ؽ�Ʈ
    public Image dayIcon;                // �� ������ UI Image (��۷� ���̱�)
    public Image nightIcon;              // �� ������ UI Image (��۷� ���̱�)
    public Image overlayImage;           // �� ��Ӱ� ó���� ��������

    public event Action<TimePhase> OnPhaseChanged;

    private float holdTimer;
    private bool isHolding;
    private bool isGameOver;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        if (holdSlider != null) holdSlider.gameObject.SetActive(false);
        ApplyPhaseUI();
    }

    void Update()
    {
        if (isGameOver) return;

        if (CurrentPhase == TimePhase.Day)
        {
            if (!isHolding && Input.GetKeyDown(KeyCode.T)) BeginHold();

            if (isHolding)
            {
                if (Input.GetKey(KeyCode.T))
                {
                    holdTimer += Time.deltaTime;
                    if (holdSlider != null) holdSlider.value = holdTimer / holdDuration;
                    if (holdTimer >= holdDuration) CompleteHold();
                }
                else if (Input.GetKeyUp(KeyCode.T)) CancelHold();
            }
        }
    }

    public void SetGameOver(bool over)
    {
        isGameOver = over;
        if (isGameOver && holdSlider != null) holdSlider.gameObject.SetActive(false);
    }

    void BeginHold()
    {
        isHolding = true;
        holdTimer = 0f;
        if (holdSlider != null)
        {
            holdSlider.gameObject.SetActive(true);
            holdSlider.value = 0f;
        }
    }

    void CancelHold()
    {
        isHolding = false;
        if (holdSlider != null) holdSlider.gameObject.SetActive(false);
    }

    void CompleteHold()
    {
        isHolding = false;
        if (holdSlider != null) holdSlider.gameObject.SetActive(false);
        SetPhase(TimePhase.Night);
    }

    public void SwitchToDay()
    {
        SetPhase(TimePhase.Day);
    }

    void SetPhase(TimePhase newPhase)
    {
        if (CurrentPhase == newPhase) return;
        CurrentPhase = newPhase;
        ApplyPhaseUI();
        OnPhaseChanged?.Invoke(newPhase);
    }

    void ApplyPhaseUI()
    {
        if (phaseText != null)
            phaseText.text = CurrentPhase == TimePhase.Day ? "DAY" : "NIGHT";

        if (dayIcon != null) dayIcon.gameObject.SetActive(CurrentPhase == TimePhase.Day);
        if (nightIcon != null) nightIcon.gameObject.SetActive(CurrentPhase == TimePhase.Night);

        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            c.a = (CurrentPhase == TimePhase.Night) ? 0.3f : 0f;
            overlayImage.color = c;
        }
    }
}
