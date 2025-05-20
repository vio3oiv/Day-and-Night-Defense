using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 밤에 몬스터를 목표치만큼 처치했을 때 보상을 클릭으로 수령하는 기능
/// </summary>
public class NightKillReward : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("밤 모드에서 처치해야 할 몬스터 수")] public int targetKills = 5;
    [Tooltip("보상 클릭을 대기할 최대 시간(초)")] public float displayDuration = 10f;

    [Header("UI 요소")]
    [Tooltip("보상 텍스트(Button)")] public Button rewardButton;
    [Tooltip("보상 안내 텍스트 (예: '5 Hearts! Click')")] public TMP_Text rewardText;

    [Header("이펙트 & 보상")]
    [Tooltip("파티클 프리팹")] public GameObject rewardParticlePrefab;

    private int killCount = 0;
    private bool isRewardActive = false;
    private Coroutine hideCoroutine;

    void OnEnable()
    {
        Monster.OnAnyMonsterKilled += HandleMonsterKilled;
        rewardButton.onClick.AddListener(ClaimReward);

        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged += HandlePhaseChanged;

        // 처음엔 숨김
        rewardButton.gameObject.SetActive(false);
        rewardText.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        Monster.OnAnyMonsterKilled -= HandleMonsterKilled;
        rewardButton.onClick.RemoveListener(ClaimReward);

        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
    }

    private void HandleMonsterKilled()
    {
        if (DayNightManager.Instance.CurrentPhase != TimePhase.Night) return;
        if (isRewardActive) return;

        killCount++;
        if (killCount >= targetKills)
            ShowReward();
    }

    private void HandlePhaseChanged(TimePhase phase)
    {
        if (phase == TimePhase.Day && isRewardActive)
            HideRewardImmediate();
    }

    private void ShowReward()
    {
        isRewardActive = true;
        // 텍스트 & 버튼 활성화
        rewardText.text = $"{targetKills}마리 제거 성공! 여기를 누르면 하트가!";
        rewardText.gameObject.SetActive(true);
        rewardButton.gameObject.SetActive(true);

        // 자동 숨김 예약
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        HideRewardImmediate();
    }

    private void ClaimReward()
    {
        // 파티클 이펙트
        if (rewardParticlePrefab != null)
            Instantiate(rewardParticlePrefab, transform.position, Quaternion.identity);

        // 하트 보상
        HeartManager.Instance.Add(targetKills);

        HideRewardImmediate();
    }

    private void HideRewardImmediate()
    {
        // 예약 코루틴 취소
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        // UI 완전 숨김
        rewardButton.gameObject.SetActive(false);
        rewardText.gameObject.SetActive(false);

        // 리셋
        isRewardActive = false;
        killCount = 0;
    }
}
