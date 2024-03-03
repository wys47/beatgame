using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCS : MonoBehaviour
{
    private WaitForSeconds[] waitForSeconds = new WaitForSeconds[1];

    public RectTransform rt;
    public RectTransform rt_t;
    public RectTransform rt_buttons;

    void Awake()
    {
        waitForSeconds[0] = new WaitForSeconds(0.01f);
    }

    public IEnumerator moveUpOrDown(int upOrdown)
    {
        for (int i = 1; i <= 10; ++i)
        {
            rt.anchoredPosition += Vector2.up * upOrdown * 60;
            rt_t.anchoredPosition += Vector2.up * upOrdown * 60;
            rt_buttons.anchoredPosition += Vector2.up * upOrdown * 60;
            yield return waitForSeconds[0];
        }
    }
}
