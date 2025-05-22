using UnityEngine;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// 버튼 OnClick에 할당해서, 인스펙터에서 인자로 넘긴 패널을 켭니다.
    /// </summary>
    public void ShowPanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(true);
    }

    /// <summary>
    /// 버튼 OnClick에 할당해서, 인스펙터에서 인자로 넘긴 패널을 끕니다.
    /// </summary>
    public void HidePanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
    }
}