using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    public int Gold { get; private set; }

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
