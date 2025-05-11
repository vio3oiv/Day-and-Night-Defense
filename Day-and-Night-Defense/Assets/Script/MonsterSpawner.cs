using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class MonsterSpawner : MonoBehaviour
{
    [Header("몬스터 설정")]
    public GameObject monsterPrefab;

    [Header("스폰 포인트")]
    [Tooltip("씬에서 몬스터를 소환할 위치들의 Transform을 드래그&드롭하세요.")]
    public Transform[] spawnPoints;

    [Header("경로 포인트")]
    [Tooltip("몬스터가 따라갈 경로의 포인트 Transform을 순서대로 드래그&드롭하세요.")]
    public Transform[] movePoints;

    [Header("웨이브 설정")]
    [Tooltip("한 번에 스폰할 몬스터 기본 개수")]
    public int startingMonstersPerWave = 3;
    [Tooltip("매 웨이브마다 증가시킬 몬스터 수")]
    public int monsterIncreasePerWave = 2;
    [Tooltip("한 번에 몬스터를 뿌릴 때의 간격(초)")]
    public float spawnInterval = 0.5f;

    [Header("UI 설정")]
    public TextMeshProUGUI waveMessageText;

    private int currentWave = 0;
    private int monstersPerWave;

    void Start()
    {
        monstersPerWave = startingMonstersPerWave;
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (true)
        {
            // 1) 밤이 될 때까지 대기
            yield return new WaitUntil(() =>
                DayNightManager.Instance.CurrentPhase == TimePhase.Night);

            // 2) 웨이브 시작: 카운트 증가 및 메시지 표시
            currentWave++;
            if (waveMessageText != null)
            {
                waveMessageText.SetText($"Wave {currentWave} Start!");
                waveMessageText.gameObject.SetActive(true);
                yield return new WaitForSeconds(2f);
                waveMessageText.gameObject.SetActive(false);
            }

            // 3) 몬스터 스폰 & 리스트 수집
            var waveMonsters = new List<Monster>();
            yield return StartCoroutine(SpawnMonsters(monstersPerWave, waveMonsters));

            // 4) 스폰된 몬스터가 모두 비활성화될 때까지 대기
            yield return new WaitUntil(() =>
                waveMonsters.All(m => m == null || !m.gameObject.activeInHierarchy));

            // 5) 웨이브 완료 → 낮 모드로 전환
            DayNightManager.Instance.SwitchToDay();

            // 6) 다음 웨이브 난이도 상승
            monstersPerWave += monsterIncreasePerWave;
        }
    }

    /// <summary>
    /// 지정된 개수만큼 모든 스폰 포인트에서 동시에 몬스터를 spawnInterval 간격으로 스폰하고,
    /// 생성된 Monster 컴포넌트를 waveMonsters 리스트에 추가합니다.
    /// </summary>
    private IEnumerator SpawnMonsters(int count, List<Monster> waveMonsters)
    {
        int spawned = 0;
        while (spawned < count)
        {
            for (int i = 0; i < spawnPoints.Length && spawned < count; i++)
            {
                Transform sp = spawnPoints[i];
                GameObject obj = Instantiate(
                    monsterPrefab,
                    sp.position,
                    Quaternion.identity
                );
                var monster = obj.GetComponent<Monster>();
                if (monster != null)
                {
                    // 이동 경로 할당
                    monster.movePoints = new List<Transform>(movePoints);
                    // 리스트에 추가
                    waveMonsters.Add(monster);
                }
                spawned++;
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// 외부에서 특정 개수의 몬스터 웨이브를 직접 트리거할 때 사용
    /// (이 경우에도 자동 낮 전환 로직은 그대로 동작합니다)
    /// </summary>
    public void SpawnWave(int count)
    {
        StopAllCoroutines();
        StartCoroutine(SpawnMonsters(count, new List<Monster>()));
    }
}
