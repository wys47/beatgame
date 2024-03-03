using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCS : MonoBehaviour
{
    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.01f);

    public RectTransform rt_b;
    public RectTransform rt_o;

    public IEnumerator enter()
    {
        for (int i = 1; i <= 5; ++i)
        {
            rt_b.anchoredPosition += Vector2.right * 140;
            yield return waitForSeconds;
        }

        for (int i = 1; i <= 5; ++i)
        {
            rt_b.anchoredPosition += Vector2.right * 140;
            rt_o.anchoredPosition += Vector2.down * 140;
            yield return waitForSeconds;
        }

        for (int i = 1; i <= 5; ++i)
        {
            rt_o.anchoredPosition += Vector2.down * 140;
            yield return waitForSeconds;
        }
    }

    public IEnumerator exit()
    {
        for (int i = 1; i <= 40; ++i)
        {
            rt_b.anchoredPosition += Vector2.right * 140;
            rt_o.anchoredPosition += Vector2.down * 140;
            yield return waitForSeconds;
        }
    }
}
