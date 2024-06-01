using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuSettingCS : Variables
{
    public GameObject[] set;
    private int currentSet;
    private const int maxSet = 2;

    [HideInInspector] public float sinc;
    public TextMeshProUGUI sincText;

    [HideInInspector] public float[] volume = new float[3];//0.마스터1.음악2.효과음
    public AudioSource[] audios;
    public SliderCS[] sliderCS;

    void OnEnable()
    {
        currentSet = 1;

        sinc = 0;
        sincText.text = "0";

        volume[0] = 0.5f;
        volume[1] = sliderCS[1].value = 0.5f;
        volume[2] = sliderCS[2].value = 0.5f;

        audios[1].volume = 0.25f;
        audios[2].volume = 0.25f;
    }

    public void changeVolume(int n, float value)
    {
        if (n == 0)
        {
            float pre = volume[0];
            volume[0] = value;

            if (pre > 0)
            {
                audios[1].volume *= value / pre;
                audios[2].volume *= value / pre;
            }
            else
            {
                audios[1].volume = value * sliderCS[1].value;
                audios[2].volume = value * sliderCS[2].value;
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
        }
    }

    public void changeSinc(int plusOrMinus)
    {
        sinc += plusOrMinus * PlusBeatInOneUpdate;

        string sign = "";
        if (sinc > 0) sign = "+";

        sincText.text = sign + sinc.ToString();
    }

    public void onLeftOrRightButtonClicked(int plusOrMinus)
    {
        currentSet += plusOrMinus;
        if (currentSet > maxSet) currentSet = 1;
        else if (currentSet == 0) currentSet = maxSet;

        for (int i = 1; i <= maxSet; ++i)
        {
            if (i == currentSet) set[i].SetActive(true);
            else set[i].SetActive(false);
        }
    }
}
