using System;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("초기 골드 설정")]
    [Tooltip("게임 시작 시 플레이어가 보유할 골드")]
    public int startingGold = 0;

    // 실제 보유 골드 (private backing)
    private int gold;
    public int Gold => gold;
    // 기존 코드 호환용
    public int CurrentGold => gold;

    public event Action<int> OnGoldChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gold = startingGold;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 시작하자마자 UI 갱신
        OnGoldChanged?.Invoke(gold);
    }

    /// <summary>
    /// 골드 증감
    /// </summary>
    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }

    /// <summary>
    /// 골드 지불 시도, 성공하면 true 반환
    /// </summary>
    public bool SpendGold(int cost)
    {
        if (gold < cost) return false;
        gold -= cost;
        OnGoldChanged?.Invoke(gold);
        return true;
    }
}
