using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    [Tooltip("한 웨이브에 스폰할 적의 수")]
    public int enemyCount;

    [Tooltip("스폰할 적 프리팹")]
    public GameObject enemyPrefab;

    [Tooltip("적을 스폰할 위치 목록")]
    public Transform[] spawnPoints;
}

public class WaveManager : MonoBehaviour
{
    [Header("웨이브 목록")]
    [Tooltip("에디터에서 Wave 요소를 추가하세요")]
    public List<Wave> waves;

    private int currentWave = 0;

    void OnEnable()
    {
        GamePhaseManager.Instance.OnPhaseChanged += HandlePhaseChange;
    }

    void OnDisable()
    {
        GamePhaseManager.Instance.OnPhaseChanged -= HandlePhaseChange;
    }

    private void HandlePhaseChange(Phase phase)
    {
        if (phase == Phase.Combat && currentWave < waves.Count)
        {
            StartCoroutine(SpawnWave(waves[currentWave]));
            currentWave++;
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            // 랜덤 스폰 위치 선택
            var points = wave.spawnPoints;
            var spawnPos = points[Random.Range(0, points.Length)].position;

            // 적 인스턴스화
            Instantiate(wave.enemyPrefab, spawnPos, Quaternion.identity);

            // 다음 스폰까지 대기
            yield return new WaitForSeconds(1f);
        }
    }
}
