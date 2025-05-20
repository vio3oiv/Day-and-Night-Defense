using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ű���� �Է� �Ŵ���: ESC Ű�� ������ UI �г��� ����մϴ�.
/// </summary>
public class KeyboardManager : MonoBehaviour
{
    public static KeyboardManager Instance { get; private set; }

    [Header("ESC ��ۿ� UI �г�")]
    [Tooltip("ESC Ű�� ������ �� ����� UI GameObject (Image, Panel ��)")]
    public GameObject escTogglePanel;

    void Awake()
    {
        // �̱��� ����
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

        // ���� �� �г� ����
        if (escTogglePanel != null)
            escTogglePanel.SetActive(false);
    }

    void Update()
    {
        // ESC Ű ������ �� ���
        if (Input.GetKeyDown(KeyCode.Escape) && escTogglePanel != null)
        {
            escTogglePanel.SetActive(!escTogglePanel.activeSelf);
        }
    }

    /// <summary>�ڵ忡�� ������ �ѱ�</summary>
    public void ShowPanel()
    {
        if (escTogglePanel != null)
            escTogglePanel.SetActive(true);
    }

    /// <summary>�ڵ忡�� ������ ����</summary>
    public void HidePanel()
    {
        if (escTogglePanel != null)
            escTogglePanel.SetActive(false);
    }
}
