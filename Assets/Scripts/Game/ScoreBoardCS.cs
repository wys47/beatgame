using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardCS : MonoBehaviour
{
    private const int preTapPoint = 100;
    private const int perfectTapPoint = 200;
    private const int tapChainPoint = 100;
    private const int clickPoint = 300;
    private const int clickChainPoint = 200;

    public TextMesh scoreText;
    public TextMesh accuracyText;

    [HideInInspector] public int nodeCnt;

    private int score;
    private float accuracy;

    private int tapCnt;
    private int tapScoreTotal;
    [HideInInspector] public bool tapAll;
    private int clickCnt;
    private int clickScoreTotal;
    [HideInInspector] public bool clickAll;

    void OnEnable()
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

    public void showScoreboard()
    {

    }
}
