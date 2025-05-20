using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// �㿡 ���͸� ��ǥġ��ŭ óġ���� �� ������ Ŭ������ �����ϴ� ���
/// </summary>
public class NightKillReward : MonoBehaviour
{
    [Header("����")]
    [Tooltip("�� ��忡�� óġ�ؾ� �� ���� ��")] public int targetKills = 5;
    [Tooltip("���� Ŭ���� ����� �ִ� �ð�(��)")] public float displayDuration = 10f;

    [Header("UI ���")]
    [Tooltip("���� �ؽ�Ʈ(Button)")] public Button rewardButton;
    [Tooltip("���� �ȳ� �ؽ�Ʈ (��: '5 Hearts! Click')")] public TMP_Text rewardText;

    [Header("����Ʈ & ����")]
    [Tooltip("��ƼŬ ������")] public GameObject rewardParticlePrefab;

    private int killCount = 0;
    private bool isRewardActive = false;
    private Coroutine hideCoroutine;

    void OnEnable()
    {
        Monster.OnAnyMonsterKilled += HandleMonsterKilled;
        rewardButton.onClick.AddListener(ClaimReward);

        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged += HandlePhaseChanged;

        // ó���� ����
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
        // �ؽ�Ʈ & ��ư Ȱ��ȭ
        rewardText.text = $"{targetKills}���� ���� ����! ���⸦ ������ ��Ʈ��!";
        rewardText.gameObject.SetActive(true);
        rewardButton.gameObject.SetActive(true);

        // �ڵ� ���� ����
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        HideRewardImmediate();
    }

    private void ClaimReward()
    {
        // ��ƼŬ ����Ʈ
        if (rewardParticlePrefab != null)
            Instantiate(rewardParticlePrefab, transform.position, Quaternion.identity);

        // ��Ʈ ����
        HeartManager.Instance.Add(targetKills);

        HideRewardImmediate();
    }

    private void HideRewardImmediate()
    {
        // ���� �ڷ�ƾ ���
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        // UI ���� ����
        rewardButton.gameObject.SetActive(false);
        rewardText.gameObject.SetActive(false);

        // ����
        isRewardActive = false;
        killCount = 0;
    }
}
