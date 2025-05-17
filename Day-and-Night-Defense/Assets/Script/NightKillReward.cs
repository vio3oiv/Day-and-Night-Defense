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
        // Monster ��ũ��Ʈ�� static event �߰� �ʿ�:
        // public static event Action OnAnyMonsterKilled;
        // Die() �޼��忡�� OnAnyMonsterKilled?.Invoke();
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
        rewardText.text = $"{targetKills} ���⸦ ������ ��Ʈ�� �þ��^0^";
        rewardButton.gameObject.SetActive(true);

        // �ڵ� ����
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        HideReward();
    }

    private void ClaimReward()
    {
        // ��ƼŬ ����Ʈ ���
        if (rewardParticlePrefab != null)
            Instantiate(rewardParticlePrefab, transform.position, Quaternion.identity);

        // ��Ʈ ����
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
