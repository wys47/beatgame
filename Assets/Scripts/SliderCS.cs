using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderCS : MonoBehaviour
{
    public SettingCS settingCS;

    public RectTransform SliderRT;
    public Transform SliderTr;

    public float minSliderPos;
    public float maxSliderPos;

    [HideInInspector] public float value;
    public int valueType;

    public void pointerDrag()
    {
        SliderTr.position = Input.mousePosition;

        SliderRT.anchoredPosition = Vector2.right * SliderRT.anchoredPosition.x;
        if (SliderRT.anchoredPosition.x < minSliderPos) SliderRT.anchoredPosition = Vector2.right * minSliderPos;
        else if (SliderRT.anchoredPosition.x > maxSliderPos) SliderRT.anchoredPosition = Vector2.right * maxSliderPos;

        value = (SliderRT.anchoredPosition.x - minSliderPos) / (maxSliderPos - minSliderPos);

        switch (valueType)
        {
            case 0:
                settingCS.ChangeVolume(0, value);
                break;
            case 1:
                settingCS.ChangeVolume(1, value);
                break;
            case 2:
                settingCS.ChangeVolume(2, value);
                break;
        }
    }
}
