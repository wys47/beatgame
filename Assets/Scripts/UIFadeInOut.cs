using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeInOut : MonoBehaviour
{
    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.01f);

    public RectTransform imageRT;
    public Image image;
    public Color color;

    void OnEnable()
    {
        image.color = color - Color.black;
    }

    public IEnumerator activate(bool isActive, Vector2 dir)
    {
        if (isActive)
        {
            image.color = color - Color.black;

            for (int i = 1; i <= 50; ++i)
            {
                image.color += Color.black * 0.02f;
                yield return waitForSeconds;
            }
        }
        else
        {
            for (int i = 1; i <= 50; ++i)
            {
                imageRT.anchoredPosition += dir * 30;
                yield return waitForSeconds;
            }
        }
    }
}
