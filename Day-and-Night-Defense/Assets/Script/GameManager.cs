using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("골드 관리")]
    public int currentGold = 0;
    public TextMeshProUGUI goldText;

    [Header("날짜/웨이브 관리")]
    [Tooltip("총 방어해야 할 날짜 수")]
    public int totalDays = 10;
    [Tooltip("각 날짜의 밤 웨이브 몬스터 수")]
    public int[] monstersPerWave;
    [Header("몬스터 스포너")]
    public MonsterSpawner monsterSpawner;
    [Header("날짜 표시 UI")]
    public TextMeshProUGUI dayText;

    /// <summary>1부터 시작하는 현재 날짜</summary>
    public int CurrentDay { get; private set; } = 1;
    public event Action<int> OnDayChanged;

    [Header("게임오버 UI")]
    public GameObject gameOverUI;

    private bool isGameOver = false;
    private int remainingMonsters = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // 1) ResourceManager의 골드 변경 이벤트를 구독
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnGoldChanged += UpdateGoldUI;
            // 초기 UI 반영
            UpdateGoldUI(ResourceManager.Instance.Gold);
        }
        else
        {
            Debug.LogWarning("[GameManager] ResourceManager 인스턴스가 없습니다.");
        }

        // 골드 UI 초기화
        //UpdateGoldUI();

        // 게임오버 UI 숨기기
        if (gameOverUI != null) gameOverUI.SetActive(false);

        // 날짜 UI 초기화
        OnDayChanged += UpdateDayUI;
        OnDayChanged?.Invoke(CurrentDay);

        // 낮/밤 전환 이벤트 구독
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged += HandlePhaseChanged;
    }

    private void UpdateGoldUI(int newGold)
    {
        if (goldText != null)
            goldText.text = $"{newGold} G";
    }

    void OnDestroy()
    {
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
        OnDayChanged -= UpdateDayUI;

        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnGoldChanged -= UpdateGoldUI;
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
        OnDayChanged -= UpdateDayUI;
    }

    /// <summary>
    /// DayNightManager에서 호출. 밤이 되면 웨이브 시작.
    /// </summary>
    private void HandlePhaseChanged(TimePhase phase)
    {
        if (isGameOver) return;
        if (phase == TimePhase.Night)
            StartNightWave();
    }

    /// <summary>
    /// 현재 날짜의 밤 웨이브를 트리거합니다.
    /// </summary>
    private void StartNightWave()
    {
        // 모든 날짜를 성공하면 클리어로
        if (CurrentDay > totalDays)
        {
            GameClear();
            return;
        }

        // 배열 범위 벗어나지 않도록 Clamp
        int idx = Mathf.Clamp(CurrentDay - 1, 0, monstersPerWave.Length - 1);
        remainingMonsters = monstersPerWave[idx];

        Debug.Log($"🌙 Day {CurrentDay} Night Wave 시작: 몬스터 {remainingMonsters}마리");

        if (monsterSpawner != null)
            monsterSpawner.SpawnWave(remainingMonsters);
        else
            Debug.LogWarning("[GameManager] monsterSpawner가 할당되지 않았습니다.");
    }

    /// <summary>
    /// 몬스터가 죽을 때마다 호출하세요.
    /// 남은 몬스터가 0이 되면 낮으로 전환하고 날짜 증가.
    /// </summary>
    public void OnMonsterKilled()
    {
        if (isGameOver) return;

        remainingMonsters--;
        if (remainingMonsters <= 0)
        {
            Debug.Log($"✅ Day {CurrentDay} Night Wave 클리어");
            DayNightManager.Instance?.SwitchToDay();

            CurrentDay++;
            OnDayChanged?.Invoke(CurrentDay);
        }
    }

    #region Gold & UI

    public void AddGold(int amount)
    {
        if (isGameOver) return;
        currentGold += amount;
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = $"{currentGold} G";
    }

    private void UpdateDayUI(int day)
    {
        if (dayText != null)
            dayText.text = $"Day {Mathf.Min(day, totalDays)}";
    }

    #endregion

    #region GameOver & Clear

    /// <summary>넥서스 파괴 시 호출</summary>
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("💀 게임 오버");
        if (gameOverUI != null) gameOverUI.SetActive(true);
    }

    private void GameClear()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("🎉 모든 날짜 방어 성공! 게임 클리어");
        // TODO: 게임 클리어 UI 표시 또는 씬 전환
    }

    #endregion

    /// <summary>
    /// 현재 씬 재로딩
    /// </summary>
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
