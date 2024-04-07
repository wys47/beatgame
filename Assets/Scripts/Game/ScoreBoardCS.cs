using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreBoardCS : MonoBehaviour
{
    private const int preTapPoint = 100;
    private const int perfectTapPoint = 200;
    private const int tapChainPoint = 100;
    private const int clickPoint = 300;
    private const int clickChainPoint = 200;

    public Image[] tile;
    private Color black = new Color(0.4f, 0.4f, 0.4f, 1f);
    public Image[] tileGlow;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI accuracyText;

    public AudioSource audioPlayer;
    public AudioClip scoreCountSound;
    public AudioClip scoreDifficaltySound;

    [HideInInspector] public int nodeCnt;

    private int score;
    private float accuracy;

    private int tapCnt;
    private int tapScoreTotal;
    [HideInInspector] public bool tapAll;
    private int clickCnt;
    private int clickScoreTotal;
    [HideInInspector] public bool clickAll;

    private WaitForSeconds[] waitForSeconds = { new WaitForSeconds(0.01f), new WaitForSeconds(0.5f), new WaitForSeconds(0.05f) };

    public void onPreTap()
    {
        score += preTapPoint;
        tapCnt = 0;
        tapScoreTotal = 0;
        tapAll = false;

        accuracy += 0.5f / nodeCnt;
    }
    public void onTapTiming(bool success)
    {
        if (success)
        {
            score += perfectTapPoint + tapChainPoint * tapCnt;
            ++tapCnt;
            tapScoreTotal += perfectTapPoint + tapChainPoint * tapCnt;

            accuracy += (float)1 / nodeCnt;
        }
        else
        {
            tapCnt = 0;
            tapScoreTotal = 0;
            tapAll = false;
        }
    }
    public void onClickTiming(bool success)
    {
        if (success)
        {
            score += clickPoint + clickChainPoint * clickCnt;
            ++clickCnt;
            clickScoreTotal += clickPoint + clickChainPoint * clickCnt;

            accuracy += (float)1 / nodeCnt;
        }
        else
        {
            clickCnt = 0;
            clickScoreTotal = 0;
            clickAll = false;
        }
    }

    public void activate()
    {
        score = 0;
        accuracy = 0;

        tapCnt = 0;
        tapScoreTotal = 0;
        tapAll = true;
        clickCnt = 0;
        clickScoreTotal = 0;
        clickAll = true;
    }

    public IEnumerator showScoreboard()
    {
        for (int i = 1; i <= 4; ++i)
        {
            tile[i].color = black;
            tileGlow[i].color = (i != 4 ? Color.white : Color.red) - Color.black;
        }

        scoreText.text = "0";
        accuracyText.text = "0%";

        audioPlayer.clip = scoreCountSound;
        audioPlayer.pitch = 1;
        audioPlayer.loop = true;
        audioPlayer.Play();
        for (int i = 0; i < score; i += 111)
        {
            yield return waitForSeconds[0];
            if (i > score) i = score;
            scoreText.text = i.ToString();
        }
        accuracy = Mathf.Round(accuracy * 1000) / 10;
        for (float f = 0; f < accuracy; f += 1.1f)
        {
            yield return waitForSeconds[0];
            if (f > accuracy) f = accuracy;
            f = Mathf.Round(f * 10) / 10;
            accuracyText.text = f.ToString() + "%";
        }
        audioPlayer.Pause();

        audioPlayer.clip = scoreDifficaltySound;
        audioPlayer.pitch = 0.5f;
        audioPlayer.loop = false;
        for (int i = 1; i <= 4; ++i)
        {

            for (int k = 1; k <= 10; ++k)
            {
                yield return waitForSeconds[0];
                if (i != 4) tile[i].color += Color.white * 0.06f;
                else tile[i].color += Color.red * 0.06f - (Color.white - Color.red) * 0.04f;
                tileGlow[i].color += Color.black * 0.1f;
            }

            audioPlayer.pitch += i != 4 ? 0.5f : 1f;
            audioPlayer.Play();
            yield return waitForSeconds[1];
            for (int k = 1; k <= 10; ++k)
            {
                yield return waitForSeconds[2];
                tileGlow[i].color = (i != 4 ? Color.white : Color.red) - Color.black * 0.1f * k;
            }
        }
    }
}
