using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 전반적인 게임 상태를 관리합니다. 넥서스 체력이 0이 되면 게임오버 UI를 표시합니다.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("골드 관리")]
    public int currentGold = 0;
    public TextMeshProUGUI goldText;

    [Header("게임오버 UI")]
    public GameObject gameOverUI;    // Inspector에서 할당

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateGoldUI();
        if (gameOverUI != null) gameOverUI.SetActive(false);
    }

    /// <summary>
    /// 골드 추가
    /// </summary>
    public void AddGold(int amount)
    {
        if (isGameOver) return;
        currentGold += amount;
        UpdateGoldUI();
    }

    void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = $"{currentGold} G";
    }

    /// <summary>
    /// 게임오버 처리: UI 활성화 및 필요한 정리
    /// </summary>
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("게임 오버!");
        if (gameOverUI != null) gameOverUI.SetActive(true);
        // Time.timeScale = 0f; // 필요에 따라 일시정지
    }
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
}
