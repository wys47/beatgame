using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variables : MonoBehaviour
{
    //게임의 주요 수치
    [HideInInspector] public const int maxTiming = 1000;
    [HideInInspector] public const int maxMapSize = 8;
    [HideInInspector] public const float PlusBeatInOneUpdate = 0.25f;
    [HideInInspector] public const int maxNodeDir = 5;
    [HideInInspector] public const int maxColor = 10;
    [HideInInspector] public const int maxFadeCnt = 5;
    [HideInInspector] public const float unpairTapNodeGeneratePosibility = 0.3f;

    //이벤트 숫자
    [HideInInspector] public int[] eventNumClickNodeRange = new int[2] { 1, 2 };
    [HideInInspector] public const int eventNumClickNodeGenerate = 1;
    [HideInInspector] public const int eventNumClickNodeAnim = 2;

    [HideInInspector] public int[] eventNumPositionedNodeRange = new int[2] { 3, 5 };

    [HideInInspector] public int[] eventNumChainNodeRange = new int[2] { 11, 13 };
    [HideInInspector] public int[] eventNumChainNodeGenerateCnt = new int[2] { 4, 8 };

    [HideInInspector] public int[] eventNumGlobalNodeRange = new int[2] { 21, 22 };
    [HideInInspector] public int[] eventNumGlobalAnim = new int[2] { 21, 22 };
}
