using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variables : MonoBehaviour
{
    //게임의 주요 수치
    protected const int maxTiming = 1000;
    protected const int maxMapSize = 8;
    protected const float PlusBeatInOneUpdate = 0.25f;
    protected const int maxNodeDir = 5;
    protected const int maxColor = 10;
    protected const int maxFadeCnt = 5;
    protected const float unpairTapNodeGeneratePosibility = 0.3f;

    //이벤트 숫자
    protected int[] eventNumClickNodeRange = { 1, 2 };
    protected const int eventNumClickNodeGenerate = 1;
    protected const int eventNumClickNodeAnim = 2;

    protected int[] eventNumPositionedNodeRange = { 3, 5 };

    protected int[] eventNumChainNodeRange = { 11, 13 };
    protected int[] eventNumChainNodeGenerate = { 0, 4, 6, 8 };
    protected const int eventChainNodeCollideTileNum = 37;

    protected int[] eventNumGlobalNodeRange1 = { 21, 22 };
    protected int[] eventNumGlobalAnim1 = { 0, 21, 22 };
}
