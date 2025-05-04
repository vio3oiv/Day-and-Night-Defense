using System.Collections;
using System.Collections.Generic;  // List 사용
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
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        while (true)
        {
            currentWave++;

            // 웨이브 시작 메시지
            if (waveMessageText != null)
            {
                waveMessageText.SetText("Wave {0} Start!", currentWave);
                waveMessageText.gameObject.SetActive(true);
                yield return new WaitForSeconds(2f);
                waveMessageText.gameObject.SetActive(false);
            }

            // 몬스터 스폰 (완료 대기)
            yield return StartCoroutine(SpawnMonsters());

            // 다음 웨이브까지 대기
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    IEnumerator SpawnMonsters()
    {
        for (int i = 0; i < monstersPerWave; i++)
        {
            if (spawnPoints.Length > 0)
            {
                int idx = Random.Range(0, spawnPoints.Length);
                // 몬스터 인스턴스화
                GameObject obj = Instantiate(
                    monsterPrefab,
                    spawnPoints[idx].position,
                    Quaternion.identity
                );

                //❗️ 여기서 movePoints 할당
                var monster = obj.GetComponent<Monster>();
                if (monster != null && movePoints.Length > 0)
                    monster.movePoints = new List<Transform>(movePoints);
            }
            else Debug.LogWarning("스폰 포인트가 없습니다!");

            yield return new WaitForSeconds(0.5f);
        }

        // 웨이브 종료 후 몬스터 수 증가
        monstersPerWave += monsterIncreasePerWave;
    }
}
