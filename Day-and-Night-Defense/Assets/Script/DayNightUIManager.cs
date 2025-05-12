using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
public class DayNightUIManager : MonoBehaviour
{
    [Header("낮 메시지")]
    public TextMeshProUGUI dayMessageText;

    [Header("밤 전환 UI")]
    public Button startNightButton;
    public GameObject nightPopup;
    public Button nightConfirmButton;
    public Button nightCancelButton;

    void Start()
    {
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged += OnPhaseChanged;

        nightPopup.SetActive(false);
        startNightButton.onClick.AddListener(() => nightPopup.SetActive(true));
        nightConfirmButton.onClick.AddListener(() =>
        {
            nightPopup.SetActive(false);
            DayNightManager.Instance.SetPhase(TimePhase.Night);
        });
        nightCancelButton.onClick.AddListener(() => nightPopup.SetActive(false));
    }

    void OnDestroy()
    {
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    private void OnPhaseChanged(TimePhase phase)
    {
        StopAllCoroutines();

        if (phase == TimePhase.Day)
        {
            StartCoroutine(DayRoutine());
            startNightButton.gameObject.SetActive(true);
        }
        else
        {
            dayMessageText.gameObject.SetActive(false);
            startNightButton.gameObject.SetActive(false);
            nightPopup.SetActive(false);
        }
    }

    private IEnumerator DayRoutine()
    {
        dayMessageText.gameObject.SetActive(true);
        dayMessageText.text = $"Day {GameManager.Instance.CurrentDay} 가 종료되었습니다.";
        yield return new WaitForSeconds(5f);
        dayMessageText.gameObject.SetActive(false);

        yield return new WaitForSeconds(10f);
        nightPopup.SetActive(true);
    }
}