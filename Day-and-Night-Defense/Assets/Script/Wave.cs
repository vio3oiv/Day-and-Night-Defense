using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    public int enemyCount;
    public GameObject enemyPrefab;
}

public class WaveManager : MonoBehaviour
{
    public List<Wave> waves;
    int currentWave = 0;

    void OnEnable()
    {
        GamePhaseManager.Instance.OnPhaseChanged += HandlePhaseChange;
    }

    void OnDisable()
    {
        GamePhaseManager.Instance.OnPhaseChanged -= HandlePhaseChange;
    }

    void HandlePhaseChange(Phase phase)
    {
        if (phase == Phase.Combat && currentWave < waves.Count)
            StartCoroutine(SpawnWave(waves[currentWave++]));
    }

    IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            Instantiate(wave.enemyPrefab, /*스폰 위치*/, Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }
    }
}
