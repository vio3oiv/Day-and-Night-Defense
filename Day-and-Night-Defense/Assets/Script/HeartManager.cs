using System;
using TMPro;
using UnityEngine;

public class HeartManager : MonoBehaviour
{
    public static HeartManager Instance { get; private set; }

    [SerializeField] private int startingHearts = 100;
    private int hearts;
    public int Hearts => hearts;

    [SerializeField] private TMP_Text heartText;

    public event Action<int> OnHeartsChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            hearts = startingHearts;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        // 초기 표시
        if (heartText)
            heartText.text = $"{hearts} ";
        // 변경 시 갱신
        OnHeartsChanged += UpdateHeartText;
    }

    void OnDisable()
    {
        OnHeartsChanged -= UpdateHeartText;
    }

    private void UpdateHeartText(int newHearts)
    {
        if (heartText)
            heartText.text = $"{newHearts} ";
    }

    public bool Spend(int amount)
    {
        if (hearts < amount) return false;
        hearts -= amount;
        OnHeartsChanged?.Invoke(hearts);
        return true;
    }

    public void Add(int amount)
    {
        hearts += amount;
        OnHeartsChanged?.Invoke(hearts);
    }
}
