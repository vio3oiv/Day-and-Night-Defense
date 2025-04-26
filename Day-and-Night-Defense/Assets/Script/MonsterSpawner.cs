using System.Collections;
using UnityEngine;
using TMPro;  // ← TMP 텍스트 사용

public class MonsterSpawner : MonoBehaviour
{
    [Header("몬스터 설정")]
    public GameObject monsterPrefab;
    public Transform[] spawnPoints; // ✨ 여러 스폰 위치 관리

    [Header("웨이브 설정")]
    public float timeBetweenWaves = 5f;
    public int monstersPerWave = 3;
    public int monsterIncreasePerWave = 2;

    [Header("UI 설정")]
    public TextMeshProUGUI waveMessageText;  // ✨ TMP 텍스트 사용

    private int currentWave = 0;
    private bool spawning = false;

    void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        while (true)
        {
            currentWave++;

            // 웨이브 시작 알림
            if (waveMessageText != null)
            {
                waveMessageText.text = $"Wave {currentWave} Start!";
                waveMessageText.gameObject.SetActive(true);
                yield return new WaitForSeconds(2f);
                waveMessageText.gameObject.SetActive(false);
            }

            // 몬스터 스폰
            StartCoroutine(SpawnMonsters());

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    IEnumerator SpawnMonsters()
    {
        spawning = true;
        for (int i = 0; i < monstersPerWave; i++)
        {
            if (spawnPoints.Length > 0)
            {
                int randIndex = Random.Range(0, spawnPoints.Length); // ✨ 랜덤 스폰포인트 선택
                Instantiate(monsterPrefab, spawnPoints[randIndex].position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("스폰 포인트가 없습니다!");
            }
            yield return new WaitForSeconds(0.5f);
        }
        spawning = false;

        // 웨이브가 끝나면 몬스터 수 증가
        monstersPerWave += monsterIncreasePerWave;
    }
}
