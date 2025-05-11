using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// 게임의 전체 흐름과 UI를 관리합니다.
/// - 골드 UI 업데이트 (ResourceManager 구독)
/// - 낮/밤 전환에 따른 몬스터 웨이브 트리거
/// - 몬스터 처치 후 낮으로 전환 및 날짜 증가
/// - 게임오버, 게임클리어 처리
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    [Tooltip("총 보유 골드를 표시할 텍스트")] public TextMeshProUGUI goldText;
    [Tooltip("현재 날짜(Day)를 표시할 텍스트")] public TextMeshProUGUI dayText;
    [Tooltip("게임 오버 시 활성화할 UI")] public GameObject gameOverUI;

    [Header("웨이브 설정")]
    [Tooltip("총 방어해야 할 날짜 수")] public int totalDays = 10;
    [Tooltip("각 날짜별 밤 웨이브 몬스터 수 배열")] public int[] monstersPerWave;

    [Header("몬스터 스포너")]
    [Tooltip("몬스터 스폰을 담당하는 컴포넌트")] public MonsterSpawner monsterSpawner;

    /// <summary>1부터 시작하는 현재 날짜</summary>
    public int CurrentDay { get; private set; } = 1;
    public event Action<int> OnDayChanged;

    private bool isGameOver = false;
    private int remainingMonsters;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // 골드 UI 갱신 구독
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnGoldChanged += UpdateGoldUI;
            UpdateGoldUI(ResourceManager.Instance.Gold);
        }
        else Debug.LogWarning("[GameManager] ResourceManager 인스턴스가 없습니다.");

        // 날짜 UI 초기화
        OnDayChanged += UpdateDayUI;
        OnDayChanged?.Invoke(CurrentDay);

        // 낮/밤 전환 이벤트 구독
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged += HandlePhaseChanged;

        // 게임오버 UI 숨기기
        if (gameOverUI != null) gameOverUI.SetActive(false);
    }

    void UpdateGoldUI(int newGold)
    {
        if (goldText != null)
            goldText.text = $"{newGold} G";
    }

    void HandlePhaseChanged(TimePhase phase)
    {
        if (isGameOver) return;
        if (phase == TimePhase.Night)
            StartNightWave();
    }

    void StartNightWave()
    {
        // 모든 날짜 방어 성공 시 클리어
        if (CurrentDay > totalDays)
        {
            GameClear();
            return;
        }

        // 몬스터 수 결정
        int idx = Mathf.Clamp(CurrentDay - 1, 0, monstersPerWave.Length - 1);
        remainingMonsters = monstersPerWave[idx];

        // 스폰 트리거
        if (monsterSpawner != null)
            monsterSpawner.SpawnWave(remainingMonsters);
        else
            Debug.LogWarning("[GameManager] MonsterSpawner가 할당되지 않았습니다.");
    }

    /// <summary>
    /// 몬스터가 죽을 때마다 호출합니다.
    /// 남은 몬스터가 0이 되면 낮으로 전환하고 날짜 증가.
    /// </summary>
    public void OnMonsterKilled()
    {
        if (isGameOver) return;

        remainingMonsters--;
        if (remainingMonsters <= 0)
        {
            DayNightManager.Instance?.SwitchToDay();
            CurrentDay++;
            OnDayChanged?.Invoke(CurrentDay);
        }
    }

    void UpdateDayUI(int day)
    {
        if (dayText != null)
            dayText.text = $"Day {Mathf.Min(day, totalDays)}";
    }

    /// <summary>넥서스 파괴 시 호출</summary>
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("💀 게임 오버");
        if (gameOverUI != null) gameOverUI.SetActive(true);
    }

    void GameClear()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("🎉 모든 날짜 방어 성공! 게임 클리어");
        // TODO: 게임 클리어 UI 또는 씬 전환
    }

    /// <summary>현재 씬 재로딩</summary>
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnGoldChanged -= UpdateGoldUI;
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
        OnDayChanged -= UpdateDayUI;
    }
}
