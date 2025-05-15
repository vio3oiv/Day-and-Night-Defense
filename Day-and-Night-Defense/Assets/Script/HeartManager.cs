using System;
using UnityEngine;

public class HeartManager : MonoBehaviour
{
    public static HeartManager Instance { get; private set; }
    [SerializeField] private int startingHearts = 100;
    private int hearts;
    public int Hearts => hearts;
    public event Action<int> OnHeartsChanged;

    void Awake()
    {
        if (Instance == null) { Instance = this; hearts = startingHearts; }
        else Destroy(gameObject);
    }

    void Start() => OnHeartsChanged?.Invoke(hearts);

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
