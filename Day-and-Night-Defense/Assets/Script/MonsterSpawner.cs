using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterSpawner : MonoBehaviour
{
    [Header("몬스터 설정")]
    public GameObject monsterPrefab;
    public Transform[] spawnPoints;   // 소환 위치

    [Header("경로 설정")]
    public Transform[] movePoints;    // 몬스터가 따라갈 경로 지점들

    [Header("웨이브 설정")]
    public float timeBetweenWaves = 5f;
    public int monstersPerWave = 3;
    public int monsterIncreasePerWave = 2;

    [Header("UI 설정")]
    public TextMeshProUGUI waveMessageText;

    private int currentWave = 0;

    void Start()
    {
        // 낮/밤 전환 이벤트 구독
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged += OnPhaseChanged;

        // 웨이브 루프 시작
        StartCoroutine(WaveRoutine());
    }

    void OnDestroy()
    {
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    private void OnPhaseChanged(TimePhase phase)
    {
        // WaveRoutine이 Night 대기 중이므로 별도 처리 불필요
    }

    private IEnumerator WaveRoutine()
    {
        while (true)
        {
            // 1) 밤이 될 때까지 대기
            yield return new WaitUntil(() => DayNightManager.Instance.CurrentPhase == TimePhase.Night);

            // 2) 웨이브 카운트 증가 및 메시지 표시
            currentWave++;
            if (waveMessageText != null)
            {
                waveMessageText.SetText("Wave {0} Start!", currentWave);
                waveMessageText.gameObject.SetActive(true);
                yield return new WaitForSeconds(2f);
                waveMessageText.gameObject.SetActive(false);
            }

            // 3) 몬스터 스폰 (현재 monstersPerWave 만큼)
            yield return StartCoroutine(SpawnMonsters(monstersPerWave));

            // 스폰 후 난이도 상승
            monstersPerWave += monsterIncreasePerWave;

            // 4) 웨이브가 끝난 뒤, 지정된 간격만큼 대기
            yield return new WaitForSeconds(timeBetweenWaves);

            // 5) 낮이 올 때까지 대기
            yield return new WaitUntil(() => DayNightManager.Instance.CurrentPhase == TimePhase.Day);
        }
    }

    /// <summary>
    /// 외부에서 특정 개수의 몬스터 웨이브를 직접 트리거할 때 사용
    /// </summary>
    public void SpawnWave(int count)
    {
        StopAllCoroutines();
        StartCoroutine(SpawnMonsters(count));
    }

    /// <summary>
    /// count 만큼 몬스터를 순차적으로 스폰합니다.
    /// </summary>
    private IEnumerator SpawnMonsters(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (spawnPoints.Length > 0)
            {
                int idx = Random.Range(0, spawnPoints.Length);
                GameObject obj = Instantiate(
                    monsterPrefab,
                    spawnPoints[idx].position,
                    Quaternion.identity
                );
                var monster = obj.GetComponent<Monster>();
                if (monster != null && movePoints.Length > 0)
                    monster.movePoints = new List<Transform>(movePoints);
            }
            else
            {
                Debug.LogWarning("스폰 포인트가 없습니다!");
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
