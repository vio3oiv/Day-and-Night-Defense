using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TimePhase { Day, Night }

/// <summary>
/// 낮/밤 전환을 관리합니다.
/// - T 키를 5초간 눌러 낮→밤 전환
/// - UI 버튼을 눌러 즉시 밤 모드로 전환
/// - 웨이브 종료 시 자동 낮으로 전환
/// UI 슬라이더, 시간대 텍스트, 아이콘 표시, 화면 어둡게 처리 포함
/// </summary>
public class DayNightManager : MonoBehaviour
{
    public static DayNightManager Instance { get; private set; }

    [Header("시간대 설정")]
    public TimePhase CurrentPhase = TimePhase.Day;
    public float holdDuration = 5f;      // T 키 누름 시간

    [Header("UI 설정")]
    public Slider holdSlider;            // T 누름 진행도 표시
    public TextMeshProUGUI phaseText;    // "DAY" / "NIGHT" 텍스트
    public Image dayIcon;                // 낮 아이콘
    public Image nightIcon;              // 밤 아이콘
    public Image overlayImage;           // 밤 어둡게 처리용 오버레이

    // 이 이벤트를 구독하면 낮/밤 전환 시 알림을 받을 수 있습니다.
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
        if (holdSlider != null)
            holdSlider.gameObject.SetActive(false);
        ApplyPhaseUI();
    }

    void Update()
    {
        if (isGameOver) return;

        // 낮일 때만 T 키로 전환 로직 동작
        if (CurrentPhase == TimePhase.Day)
        {
            if (!isHolding && Input.GetKeyDown(KeyCode.T))
                BeginHold();

            if (isHolding)
            {
                if (Input.GetKey(KeyCode.T))
                {
                    holdTimer += Time.deltaTime;
                    if (holdSlider != null)
                        holdSlider.value = holdTimer / holdDuration;
                    if (holdTimer >= holdDuration)
                        CompleteHold();
                }
                else if (Input.GetKeyUp(KeyCode.T))
                {
                    CancelHold();
                }
            }
        }
    }

    /// <summary>
    /// UI 버튼에서 직접 밤 모드로 전환할 때 호출하세요.
    /// </summary>
    public void StartNight()
    {
        if (isGameOver) return;
        if (CurrentPhase == TimePhase.Day)
            CompleteHold();
    }

    public void SwitchToDay()
    {
        SetPhase(TimePhase.Day);
    }

    public void SetGameOver(bool over)
    {
        isGameOver = over;
        if (isGameOver && holdSlider != null)
            holdSlider.gameObject.SetActive(false);
    }

    // T 키 누르기 시작
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

    // 누름 취소
    void CancelHold()
    {
        isHolding = false;
        if (holdSlider != null)
            holdSlider.gameObject.SetActive(false);
    }

    // 누름 완료 → 밤 전환
    void CompleteHold()
    {
        isHolding = false;
        if (holdSlider != null)
            holdSlider.gameObject.SetActive(false);
        SetPhase(TimePhase.Night);
    }

    /// <summary>
    /// 실제로 낮/밤을 전환합니다.
    /// </summary>
    public void SetPhase(TimePhase newPhase)
    {
        if (CurrentPhase == newPhase) return;
        CurrentPhase = newPhase;
        ApplyPhaseUI();
        OnPhaseChanged?.Invoke(newPhase);
    }

    // UI(텍스트, 아이콘, 오버레이) 갱신
    void ApplyPhaseUI()
    {
        if (phaseText != null)
            phaseText.text = (CurrentPhase == TimePhase.Day) ? "DAY" : "NIGHT";

        if (dayIcon != null)
            dayIcon.gameObject.SetActive(CurrentPhase == TimePhase.Day);
        if (nightIcon != null)
            nightIcon.gameObject.SetActive(CurrentPhase == TimePhase.Night);

        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            c.a = (CurrentPhase == TimePhase.Night) ? 0.3f : 0f;
            overlayImage.color = c;
        }
    }
}
