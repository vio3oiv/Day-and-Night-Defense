using System.Collections;
using UnityEngine;
using TMPro;

public class DayDialogue : MonoBehaviour
{
    [Header("��� ����")]
    [Tooltip("���� ������� ������ ����")]
    public string[] dialogues;
    [Tooltip("��� ������ �ð� ����(��)")]
    public float interval = 5f;

    [Header("UI ����")]
    public TMP_Text dialogueText;

    private Coroutine cycleCoroutine;

    void Start()
    {
        // Start������ ���� ������ üũ
        if (DayNightManager.Instance != null)
        {
            DayNightManager.Instance.OnPhaseChanged += HandlePhaseChanged;
            HandlePhaseChanged(DayNightManager.Instance.CurrentPhase);
        }
        else
        {
            Debug.LogWarning("[DayDialogue] DayNightManager �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
    }

    void OnDisable()
    {
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
        StopCycling();
    }

    private void HandlePhaseChanged(TimePhase phase)
    {
        Debug.Log($"[DayDialogue] Phase changed to {phase}");
        if (phase == TimePhase.Day)
        {
            dialogueText.gameObject.SetActive(true);
            StartCycling();
        }
        else
        {
            StopCycling();
            dialogueText.gameObject.SetActive(false);
        }
    }

    private void StartCycling()
    {
        if (cycleCoroutine == null && dialogues != null && dialogues.Length > 0)
            cycleCoroutine = StartCoroutine(CycleDialogues());
    }

    private void StopCycling()
    {
        if (cycleCoroutine != null)
        {
            StopCoroutine(cycleCoroutine);
            cycleCoroutine = null;
        }
    }

    private IEnumerator CycleDialogues()
    {
        int idx = 0;
        while (true)
        {
            if (dialogues.Length == 0)
            {
                Debug.LogWarning("[DayDialogue] dialogues �迭�� ��� �ֽ��ϴ�.");
                yield break;
            }

            dialogueText.text = dialogues[idx];
            Debug.Log($"[DayDialogue] Show dialogue: {dialogues[idx]}");
            idx = (idx + 1) % dialogues.Length;
            yield return new WaitForSeconds(interval);
        }
    }
}
