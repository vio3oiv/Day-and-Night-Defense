using System;
using UnityEngine;
using UnityEngine.UI;

public enum TimePhase { Day, Night }

/// <summary>
/// 낮/밤 전환을 관리합니다. T 키를 5초간 눌러 낮→밤 전환, 웨이브 종료 시 자동 낮으로 전환
/// UI 슬라이더, 시간대 표시, 화면 어둡게 처리 포함
/// </summary>
public class DayNightManager : MonoBehaviour
{
    public static DayNightManager Instance { get; private set; }

    [Header("시간대 설정")]
    public TimePhase CurrentPhase = TimePhase.Day;
    public float holdDuration = 5f;      // T 키 누름 시간

    [Header("UI 설정")]
    public Slider holdSlider;            // T 누름 진행도 표시
    public Text phaseText;               // "DAY" / "NIGHT" 표시
    public Image overlayImage;           // 밤 어둡게 처리용 오버레이

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
        // 초기 UI 세팅
        if (holdSlider != null) holdSlider.gameObject.SetActive(false);
        ApplyPhaseUI();
    }

    void Update()
    {
        if (isGameOver) return;

        // 낮 모드에서만 T 키 입력 받아 밤으로 전환 시도
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
    /// 파라미터로 게임오버 여부 설정 (추가 시 사용)
    /// </summary>
    public void SetGameOver(bool over)
    {
        isGameOver = over;
        if (isGameOver && holdSlider != null)
            holdSlider.gameObject.SetActive(false);
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
        if (holdSlider != null)
            holdSlider.gameObject.SetActive(false);
    }

    void CompleteHold()
    {
        isHolding = false;
        if (holdSlider != null)
            holdSlider.gameObject.SetActive(false);

        // 낮 → 밤 전환
        SetPhase(TimePhase.Night);
    }

    /// <summary>
    /// 외부(웨이브 매니저)에서 호출: 밤 웨이브 종료 시 낮으로 전환
    /// </summary>
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

        if (overlayImage != null)
        {
            // 밤일 때 살짝 어둡게
            Color c = overlayImage.color;
            c.a = (CurrentPhase == TimePhase.Night) ? 0.3f : 0f;
            overlayImage.color = c;
        }
    }
}
