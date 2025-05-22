using UnityEngine;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// ��ư OnClick�� �Ҵ��ؼ�, �ν����Ϳ��� ���ڷ� �ѱ� �г��� �մϴ�.
    /// </summary>
    public void ShowPanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(true);
    }

    /// <summary>
    /// ��ư OnClick�� �Ҵ��ؼ�, �ν����Ϳ��� ���ڷ� �ѱ� �г��� ���ϴ�.
    /// </summary>
    public void HidePanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
    }
}