using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }
    private readonly HashSet<string> installedTowers = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterTower(string typeID) => installedTowers.Add(typeID);
    public void UnregisterTower(string typeID) => installedTowers.Remove(typeID);
    public bool HasTower(string typeID) => installedTowers.Contains(typeID);
}
