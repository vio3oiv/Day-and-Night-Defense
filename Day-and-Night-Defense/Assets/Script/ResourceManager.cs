using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    // 내부에서만 변경 가능한 실제 골드 값
    public int Gold { get; private set; }

    // 기존 Tower 스크립트의 CurrentGold 호출을 위해 추가
    public int CurrentGold => Gold;

    public event Action<int> OnGoldChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        OnGoldChanged?.Invoke(Gold);
    }

    public bool SpendGold(int cost)
    {
        if (Gold < cost) return false;
        Gold -= cost;
        OnGoldChanged?.Invoke(Gold);
        return true;
    }
}
