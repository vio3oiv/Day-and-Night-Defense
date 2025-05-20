using System.Collections;
using UnityEngine;
using TMPro;

public class DayDialogue : MonoBehaviour
{
    [Header("대사 설정")]
    [Tooltip("낮에 순서대로 보여줄 대사들")]
    public string[] dialogues;
    [Tooltip("대사 사이의 시간 간격(초)")]
    public float interval = 5f;

    [Header("UI 참조")]
    public TMP_Text dialogueText;

    private Coroutine cycleCoroutine;

    void Start()
    {
        // Start에서도 현재 페이즈 체크
        if (DayNightManager.Instance != null)
        {
            DayNightManager.Instance.OnPhaseChanged += HandlePhaseChanged;
            HandlePhaseChanged(DayNightManager.Instance.CurrentPhase);
        }
        else
        {
            Debug.LogWarning("[DayDialogue] DayNightManager 인스턴스를 찾을 수 없습니다.");
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
                Debug.LogWarning("[DayDialogue] dialogues 배열이 비어 있습니다.");
                yield break;
            }

            dialogueText.text = dialogues[idx];
            Debug.Log($"[DayDialogue] Show dialogue: {dialogues[idx]}");
            idx = (idx + 1) % dialogues.Length;
            yield return new WaitForSeconds(interval);
        }
    }
}
