using UnityEngine;
using System;

public enum Phase { Build, Combat }

public class GamePhaseManager : MonoBehaviour
{
    public static GamePhaseManager Instance { get; private set; }
    public Phase CurrentPhase { get; private set; }

    public event Action<Phase> OnPhaseChanged;

    [Header("타이머 설정")]
    public float buildDuration = 60f;
    public float combatDuration = 30f;
    float phaseTimer;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        EnterPhase(Phase.Build);
    }

    void Update()
    {
        phaseTimer -= Time.deltaTime;
        if (phaseTimer <= 0f)
        {
            EnterPhase(CurrentPhase == Phase.Build ? Phase.Combat : Phase.Build);
        }
    }

    void EnterPhase(Phase newPhase)
    {
        CurrentPhase = newPhase;
        phaseTimer = (newPhase == Phase.Build ? buildDuration : combatDuration);
        OnPhaseChanged?.Invoke(newPhase);
        // (예) 카메라 배경색, UI 변경 등 처리
    }
}
