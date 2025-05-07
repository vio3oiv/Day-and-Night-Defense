using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DayDisplay : MonoBehaviour
{
    TextMeshProUGUI _text;

    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        // 게임매니저 이벤트 구독
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDayChanged += UpdateDayText;
            // 초기 표시
            UpdateDayText(GameManager.Instance.CurrentDay);
        }
    }

    void UpdateDayText(int day)
    {
        _text.text = $"Day {day}";
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnDayChanged -= UpdateDayText;
    }
}
