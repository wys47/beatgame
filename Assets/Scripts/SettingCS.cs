using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingCS : MonoBehaviour
{
    [HideInInspector] public float sinc = -0.25f;

    [HideInInspector] public float[] volume = new float[3];//0.마스터1.게임2.메뉴
    public AudioSource[] audios;
    public SliderCS[] sliderCS;

    void OnEnable()
    {
        volume[0] = 0.5f;
        volume[1] = sliderCS[1].value = 0.5f;
        volume[2] = sliderCS[2].value = 0.5f;

        audios[1].volume = 0.25f;
        audios[2].volume = 0.25f;
        audios[3].volume = 0.25f;
    }

    public void ChangeVolume(int n, float value)
    {
        if (n == 0)
        {
            float pre = volume[0];
            volume[0] = value;

            if (pre > 0)
            {
                audios[1].volume *= value / pre;
                audios[2].volume *= value / pre;
                audios[3].volume *= value / pre;
            }
            else
            {
                audios[1].volume = value * sliderCS[1].value;
                audios[2].volume = value * sliderCS[2].value;
                audios[3].volume = value * sliderCS[2].value;
            }
        }
        else if (n == 1)
        {
            volume[1] = volume[0] * value;
            audios[1].volume = volume[1];
        }
        else if (n == 2)
        {
            volume[2] = volume[0] * value;
            audios[2].volume = volume[2];
            audios[3].volume = volume[2];
        }
    }
}
