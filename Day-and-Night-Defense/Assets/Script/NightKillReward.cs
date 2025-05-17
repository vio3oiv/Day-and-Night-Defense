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
        // Monster 스크립트에 static event 추가 필요:
        // public static event Action OnAnyMonsterKilled;
        // Die() 메서드에서 OnAnyMonsterKilled?.Invoke();
        Monster.OnAnyMonsterKilled += HandleMonsterKilled;

        rewardButton.gameObject.SetActive(false);
        rewardButton.onClick.AddListener(ClaimReward);
    }

    void OnDisable()
    {
        Monster.OnAnyMonsterKilled -= HandleMonsterKilled;
        rewardButton.onClick.RemoveListener(ClaimReward);
    }

    private void HandleMonsterKilled()
    {
        if (DayNightManager.Instance.CurrentPhase != TimePhase.Night) return;

        killCount++;
        if (!isRewardActive && killCount >= targetKills)
            ShowReward();
    }

    private void ShowReward()
    {
        isRewardActive = true;
        rewardText.text = $"{targetKills} 여기를 누르면 하트가 늘어나요^0^";
        rewardButton.gameObject.SetActive(true);

        // 자동 숨김
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        HideReward();
    }

    private void ClaimReward()
    {
        // 파티클 이펙트 재생
        if (rewardParticlePrefab != null)
            Instantiate(rewardParticlePrefab, transform.position, Quaternion.identity);

        // 하트 보상
        HeartManager.Instance.Add(targetKills);

        HideReward();
    }

    private void HideReward()
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        rewardButton.gameObject.SetActive(false);
        isRewardActive = false;
        killCount = 0;
    }
}
