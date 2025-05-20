using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 키보드 입력 매니저: ESC 키로 지정된 UI 패널을 토글합니다.
/// </summary>
public class KeyboardManager : MonoBehaviour
{
    public static KeyboardManager Instance { get; private set; }

    [Header("ESC 토글용 UI 패널")]
    [Tooltip("ESC 키를 눌렀을 때 토글할 UI GameObject (Image, Panel 등)")]
    public GameObject escTogglePanel;

    void Awake()
    {
        // 싱글턴 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 시작 시 패널 숨김
        if (escTogglePanel != null)
            escTogglePanel.SetActive(false);
    }

    void Update()
    {
        // ESC 키 눌렀을 때 토글
        if (Input.GetKeyDown(KeyCode.Escape) && escTogglePanel != null)
        {
            escTogglePanel.SetActive(!escTogglePanel.activeSelf);
        }
    }

    /// <summary>코드에서 강제로 켜기</summary>
    public void ShowPanel()
    {
        if (escTogglePanel != null)
            escTogglePanel.SetActive(true);
    }

    /// <summary>코드에서 강제로 끄기</summary>
    public void HidePanel()
    {
        if (escTogglePanel != null)
            escTogglePanel.SetActive(false);
    }
}
